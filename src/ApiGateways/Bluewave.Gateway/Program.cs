var builder = WebApplication.CreateBuilder(args);

// Add the YARP service reading from appsettings
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Activates the YARP middleware
app.MapReverseProxy();

app.Run();