using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration)
    .AddPolly();

// Add controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Gateway",
        Version = "v1"
    });
});

var app = builder.Build();

// Add a test endpoint before Ocelot
app.MapGet("/", () => "API Gateway is running! Try /api/jobportal or /health/jobportal");
app.MapGet("/test", () => "Gateway test endpoint is working!");

// Add health check endpoint
app.MapGet("/health", () => new
{
    Status = "Healthy",
    Gateway = "API Gateway",
    Timestamp = DateTime.UtcNow,
    Services = new[]
    {
        "JobPortal: http://localhost:5002",
        "Marketplace: http://localhost:5003",
        "LocalMarket: http://localhost:5004"
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Use Ocelot middleware
await app.UseOcelot();

app.Run();