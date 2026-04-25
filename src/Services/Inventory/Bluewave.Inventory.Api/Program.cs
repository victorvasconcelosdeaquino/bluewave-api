using Bluewave.Core.Extensions;
using Bluewave.Inventory.Application.Features.ProductCategory.Commands.CreateCategory;
using System.Reflection;
using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Application.Features.Stock.Consumers;
using Bluewave.Inventory.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.Retry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(builder.Configuration["OTEL_SERVICE_NAME"] ?? "inventory-api"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddSource("MassTransit")
            .AddOtlpExporter();
    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Bluewave Inventory API", Version = "v1" });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Host=localhost;Port=5433;Database=bluewave;Username=admin;Password=admin;";

builder.Services.AddDbContext<InventoryDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
    });
    options.UseSnakeCaseNamingConvention();
});

builder.Services.AddScoped<IInventoryDbContext>(provider => provider.GetRequiredService<InventoryDbContext>());

builder.Services.AddBluewaveCore(typeof(CreateCategoryCommandValidator).Assembly);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMqHost"] ?? "localhost";

        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("stock-order-created", e =>
        {
            e.UseMessageRetry(r => r.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5)));

            e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
    });
});

var app = builder.Build();

app.UseBluewaveCore();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<InventoryDbContext>();

    // Resiliency with Polly
    var pipeline = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder().Handle<Exception>(),
            MaxRetryAttempts = 5,
            Delay = TimeSpan.FromSeconds(3),
            BackoffType = DelayBackoffType.Constant,
            OnRetry = args =>
            {
                logger.LogWarning($"Connection failed: {args.Outcome.Exception?.Message}. Try {args.AttemptNumber} of 5...");
                return ValueTask.CompletedTask;
            }
        })
        .Build();

    pipeline.Execute(() =>
    {
        logger.LogInformation("(Polly) Starting database migration...");
        context.Database.Migrate();
        logger.LogInformation("DATABASE MIGRATED SUCCESSFULLY!");
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// In production scenarios, it's recommended to enable HTTPS redirection
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();