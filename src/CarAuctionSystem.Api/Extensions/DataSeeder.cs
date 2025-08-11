using CarAuctionSystem.Application.DTOs;
using CarAuctionSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CarAuctionSystem.Api.Extensions;

public static class DataSeeder
{
    public static async Task SeedData(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var auctionService = scope.ServiceProvider.GetRequiredService<IAuctionService>();

        // Seed Vehicles with their auctions
        var vehicleRequests = new List<CreateVehicleRequest>
        {
            new CreateVehicleRequest("001","S001", "Sedan", "Toyota", "Camry", 2023, 25000, "USD", 
                        new Dictionary<string, object> { ["NumberOfDoors"] = 4 }),
            new CreateVehicleRequest("002", "SUV001", "SUV", "Honda", "Pilot", 2022, 35000, "USD", 
                        new Dictionary<string, object> { ["NumberOfSeats"] = 8 }),
            new CreateVehicleRequest("003", "T001", "Truck", "Ford", "F-150", 2021, 45000, "USD", 
                        new Dictionary<string, object> { ["LoadCapacity"] = 1000.5m }),
            new CreateVehicleRequest("004", "H001", "Hatchback", "Volkswagen", "Golf", 2023, 22000, "USD", 
                        new Dictionary<string, object> { ["NumberOfDoors"] = 5 })
        };

        foreach (var request in vehicleRequests)
        {
            // Create vehicle
            var vehicle = await auctionService.AddVehicleAsync(request, cancellationToken);

            // Start auction for the vehicle
            // var auctionRequest = new StartAuctionRequest
            // {
            //     VehicleId = vehicle.Id,
            //     StartingPrice = vehicle.BasePrice * 0.9m, // Starting bid at 90% of base price
            //     EndDate = DateTime.UtcNow.AddDays(7) // 7-day auction
            // };

            var auctionRequest = new StartAuctionRequest(vehicle.Id);
            
            await auctionService.StartAuctionAsync(auctionRequest, cancellationToken);
        }
        
        // Log successful seeding
        Console.WriteLine("Successfully seeded initial data");
    }
}
