using Bluewave.Core.Extensions;
using Bluewave.Sales.Application.Features.Orders.Commands.CreateOrder;
using Bluewave.Sales.Application.Interfaces;
using Bluewave.Sales.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.Retry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// OPEN TELEMETRY
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(builder.Configuration["OTEL_SERVICE_NAME"] ?? "sales-api"))
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
    c.SwaggerDoc("v1", new() { Title = "Bluewave Sales API", Version = "v1" });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Host=localhost;Port=5433;Database=bluewave_sales;Username=admin;Password=admin;";

builder.Services.AddDbContext<SalesDbContext>(options =>
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

builder.Services.AddScoped<ISalesDbContext>(provider => provider.GetRequiredService<SalesDbContext>());

builder.Services.AddBluewaveCore(typeof(CreateOrderCommand).Assembly);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMqHost"] ?? "localhost";

        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

var app = builder.Build();

app.UseBluewaveCore();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<SalesDbContext>();

    var pipeline = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder().Handle<Exception>(),
            MaxRetryAttempts = 5,
            Delay = TimeSpan.FromSeconds(3),
            BackoffType = DelayBackoffType.Constant,
            OnRetry = args =>
            {
                logger.LogWarning($"Failed to connect to Sales Database: {args.Outcome.Exception?.Message}. Attempt {args.AttemptNumber} of 5...");
                return ValueTask.CompletedTask;
            }
        })
        .Build();

    pipeline.Execute(() =>
    {
        logger.LogInformation("(Polly) Starting Sales Database migration...");
        db.Database.Migrate();
        logger.LogInformation("Sales Database migrated successfully!");
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// In production scenarios, it's recommended to enable HTTPS redirection
// app.UseHttpsRedirection(); 

app.UseAuthorization();

app.MapControllers();

app.Run();