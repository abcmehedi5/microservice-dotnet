using FreelanceMarketplaceService.API.Extensions;
using FreelanceMarketplaceService.API.gRPC;
using FreelanceMarketplaceService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel for multiple endpoints
builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP endpoint (for REST API)
    options.ListenLocalhost(5003, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
    });

    // gRPC endpoint
    options.ListenLocalhost(5006, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});

// Add services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Map gRPC service
app.MapGrpcService<MarketplaceGrpcService>();

// Map health checks
app.MapHealthChecks("/health");

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();