using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// OPEN TELEMETRY
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(builder.Configuration["OTEL_SERVICE_NAME"] ?? "gateway"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation() // Tracks outgoing HTTP requests to apis
            .AddOtlpExporter();
    });

// Add the YARP service reading from appsettings
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Activates the YARP middleware
app.MapReverseProxy();

app.Run();