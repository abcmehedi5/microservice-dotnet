using FreelanceMarketplaceService.Application.Mappings;
using FreelanceMarketplaceService.Application.Services;
using FreelanceMarketplaceService.Core.Interfaces;
using FreelanceMarketplaceService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FreelanceMarketplaceService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Register application services
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IFreelancerService, FreelancerService>();

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<MarketplaceDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("MarketplaceDb")));

            return services;
        }

        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add controllers
            services.AddControllers();

            // Add Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Freelance Marketplace Service API",
                    Version = "v1",
                    Description = "Microservice for Freelance Marketplace functionality"
                });
            });

            // Add gRPC
            services.AddGrpc();

            // Add CORS for development
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Add Health Checks
            //services.AddHealthChecks()
            //    .AddDbContextCheck<MarketplaceDbContext>();

            return services;
        }
    }
}