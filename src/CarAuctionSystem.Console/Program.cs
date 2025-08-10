using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CarAuctionSystem.Application.DTOs;
using CarAuctionSystem.Application.Services;
using CarAuctionSystem.Console.Commands;
using CarAuctionSystem.Infrastructure.Extensions;

namespace CarAuctionSystem.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var app = host.Services.GetRequiredService<ConsoleApplication>();
            await app.RunAsync();
        }
        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddCarAuctionSystem();
                    services.AddScoped<ConsoleApplication>();
                    services.AddScoped<ICommandHandler, CommandHandler>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Information);
                });
    }

    public class ConsoleApplication
    {
        private readonly IAuctionService _auctionService;
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<ConsoleApplication> _logger;

        public ConsoleApplication(IAuctionService auctionService, ICommandHandler commandHandler, ILogger<ConsoleApplication> logger)
        {
            _auctionService = auctionService;
            _commandHandler = commandHandler;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            System.Console.WriteLine("---- Car Auction Management System ----");
            System.Console.WriteLine("Type 'help' for a list of commands.");
            await SeedSampleDataAsync();
            while (true)
            {
                try
                {
                    await DisplayMainMenu();
                    var input = System.Console.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(input))
                        continue;
                    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                        break;
                    await _commandHandler.HandleCommandAsync(input);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error: {ex.Message}");
                    _logger.LogError(ex, "An error occurred while processing the command.");
                }
                System.Console.WriteLine();
            }
            System.Console.WriteLine("Thanks for using the Car Auction Management System!");
        }

        private async Task DisplayMainMenu()
        {
            var vehicleCount = await _auctionService.GetVehicleCountAsync();
            var activeAuctionCount = await _auctionService.GetActiveAuctionCountAsync();

            System.Console.WriteLine($"Current Status: {vehicleCount} vehicles, {activeAuctionCount} active auctions");
            System.Console.WriteLine();
            System.Console.WriteLine("Available Commands:");
            System.Console.WriteLine("1. add-vehicle    - Add a new vehicle to inventory");
            System.Console.WriteLine("2. search         - Search for vehicles");
            System.Console.WriteLine("3. start-auction  - Start an auction for a vehicle");
            System.Console.WriteLine("4. place-bid      - Place a bid on an active auction");
            System.Console.WriteLine("5. close-auction  - Close an active auction");
            System.Console.WriteLine("6. show-auctions  - Show all active auctions");
            System.Console.WriteLine("7. show-types     - Show supported vehicle types");
            System.Console.WriteLine("8. help          - Show detailed command help");
            System.Console.WriteLine("exit             - Exit the application");
            System.Console.WriteLine();
            System.Console.Write("Enter command: ");
        }

        private async Task SeedSampleDataAsync()
        {
            try
            {
                // Add sample vehicles
                var sampleVehicles = new[]
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

                foreach (var vehicle in sampleVehicles)
                {
                    try
                    {
                        await _auctionService.AddVehicleAsync(vehicle);
                    }
                    catch (Exception ex) when (ex.Message.Contains("already exists"))
                    {
                        // Vehicle already exists, skip
                    }
                }

                _logger.LogInformation("Sample data seeded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to seed sample data");
            }
        }
    }
}