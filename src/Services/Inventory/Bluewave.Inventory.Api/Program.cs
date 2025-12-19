using Bluewave.Inventory.Application; // We'll need to create an extension method here later
using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Application.Features.Stock.Consumers;
using Bluewave.Inventory.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
// If you've created validation classes or MediatR handlers, register them here.

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// 1. SERVICE CONFIGURATION (DI)
// ============================================================================

// Adds Controllers
builder.Services.AddControllers();

// Configures Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Bluewave Inventory API", Version = "v1" });
});

// --- DATABASE (PostgreSQL) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<InventoryDbContext>(options =>
{
    options.UseNpgsql(connectionString);

    // Important: Transforms 'CompanyName' (C#) to 'company_name' (Postgres)
    options.UseSnakeCaseNamingConvention();
});

builder.Services.AddScoped<IInventoryDbContext>(provider => provider.GetRequiredService<InventoryDbContext>());

// --- MESSAGING (RabbitMQ with MassTransit) ---
builder.Services.AddMassTransit(x =>
{
    // Register the consumer
    x.AddConsumer<OrderCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        // Configure the conection using the docker-compose credentials
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Create automatically the queue and conect it to the consumer
        cfg.ReceiveEndpoint("stock-order-created", e =>
        {
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
    });
});

// --- APPLICATION LAYER (MediatR) ---
var applicationAssembly = typeof(Bluewave.Inventory.Application.DependencyInjection).Assembly;
builder.Services.AddApplication(); // Using the extension method we created

// --- INFRASTRUCTURE LAYER (Bus, Cache, Repositories) ---


// ============================================================================
// 2. BUILD THE APP
// ============================================================================
var app = builder.Build();

// ============================================================================
// 3. REQUEST PIPELINE AND AUTOMATIC MIGRATION
// ============================================================================

// Applies Migrations automatically on startup (Great for Docker/Dev)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<InventoryDbContext>();

        // 1. Applies Migrations (Creates tables if they don't exist)
        context.Database.Migrate();
        Console.WriteLine("Database migrated successfully!");

        // 2. Runs the Seed (Populates initial data)
        DbInitializer.Seed(context);
        Console.WriteLine("Initial seed data executed!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing the database: {ex.Message}");
    }
}

// Configure Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Quick endpoint just to see if the data is there (Minimal API)
app.MapGet("/api/test-data", async (InventoryDbContext db) =>
{
    return await db.Products
        .Include(p => p.Category)
        .Include(p => p.Uom)
        .ToListAsync();
});

app.Run();