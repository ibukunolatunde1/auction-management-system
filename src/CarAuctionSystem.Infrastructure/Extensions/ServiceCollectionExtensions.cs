using Microsoft.Extensions.DependencyInjection;
using CarAuctionSystem.Application.Factories;
using CarAuctionSystem.Application.Interfaces.Repositories;
using CarAuctionSystem.Application.Services;
using CarAuctionSystem.Infrastructure.Repositories;

namespace CarAuctionSystem.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCarAuctionInfrastructure(this IServiceCollection services)
        {
            // Register repositories
            services.AddSingleton<IVehicleRepository, InMemoryVehicleRepository>();
            services.AddSingleton<IAuctionRepository, InMemoryAuctionRepository>();
            
            return services;
        }

        public static IServiceCollection AddCarAuctionApplication(this IServiceCollection services)
        {
            // Register application services
            services.AddScoped<IAuctionService, AuctionService>();
            services.AddScoped<IVehicleFactory, VehicleFactory>();
            
            return services;
        }

        public static IServiceCollection AddCarAuctionSystem(this IServiceCollection services)
        {
            return services
                .AddCarAuctionInfrastructure()
                .AddCarAuctionApplication();
        }
    }
}