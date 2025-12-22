using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Application.Features.Stock.Consumers;
using Bluewave.Inventory.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Basisc Configurations
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Bluewave Inventory API", Version = "v1" });
});

// 2. Database (PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Host=localhost;Port=5433;Database=bluewave;Username=admin;Password=admin;";

builder.Services.AddDbContext<InventoryDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.UseSnakeCaseNamingConvention();
});

builder.Services.AddScoped<IInventoryDbContext>(provider => provider.GetRequiredService<InventoryDbContext>());

// 3. MediatR (Application layer)
// Manuelly register to ensure Docker can find the Handlers
builder.Services.AddMediatR(cfg =>
{
    // Uses the consumer as a reference to find the Application assembly
    cfg.RegisterServicesFromAssembly(typeof(OrderCreatedConsumer).Assembly);
});

// 4. MassTransit (RabbitMQ)
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
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
    });
});

var app = builder.Build();

// 5. Automatic Migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<InventoryDbContext>();
        context.Database.Migrate();
        Console.WriteLine("Database (Inventory) migrated successfully!");

        // DbInitializer.Seed(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing database: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();