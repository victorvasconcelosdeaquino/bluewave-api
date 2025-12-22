using Bluewave.Sales.Application.Common.Behaviors;
using Bluewave.Sales.Application.Features.Orders.Commands.CreateOrder;
using Bluewave.Sales.Application.Interfaces;
using Bluewave.Sales.Infrastructure.Persistence;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Bluewave Sales API", Version = "v1" });
});

// 1. Database (PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Host=localhost;Port=5433;Database=bluewave_sales;Username=admin;Password=admin;";

builder.Services.AddDbContext<SalesDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.UseSnakeCaseNamingConvention();
});

builder.Services.AddScoped<ISalesDbContext>(provider => provider.GetRequiredService<SalesDbContext>());

// 2. MediatR (Application)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));

// 3. MassTransit (RabbitMQ)
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
        // --------------------
    });
});

//4. Pipeline Behavior
// FluentValidation configuration to register all validators from the Application assembly
builder.Services.AddValidatorsFromAssembly(typeof(CreateOrderCommand).Assembly);

// MediatR configuration to register services and add the ValidationBehavior
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly);

    // Adds the ValidationBehavior to the MediatR pipeline
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

var app = builder.Build();

// 5. Global Exception Handler Middleware
app.UseMiddleware<Bluewave.Sales.Api.Middlewares.GlobalExceptionHandlerMiddleware>();

// 6. Migração Automática (Cria o banco sales se não existir)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<SalesDbContext>();
        db.Database.Migrate();
        Console.WriteLine("✅ Banco de Vendas migrado com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao migrar Vendas: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Endpoint Rápido (Minimal API) para testar
app.MapPost("/api/orders", async (IMediator mediator, CreateOrderCommand command) =>
{
    var id = await mediator.Send(command);
    return Results.Created($"/api/orders/{id}", id);
});

app.Run();