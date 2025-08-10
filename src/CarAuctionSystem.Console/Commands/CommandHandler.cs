using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarAuctionSystem.Application.DTOs;
using CarAuctionSystem.Application.Services;

namespace CarAuctionSystem.Console.Commands
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IAuctionService _auctionService;

        public CommandHandler(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        public async Task HandleCommandAsync(string command)
        {
            var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            var commandName = parts[0].ToLowerInvariant();

            switch (commandName)
            {
                case "add-vehicle":
                case "1":
                    await HandleAddVehicleAsync();
                    break;
                case "search":
                case "2":
                    await HandleSearchAsync();
                    break;
                case "start-auction":
                case "3":
                    await HandleStartAuctionAsync();
                    break;
                case "place-bid":
                case "4":
                    await HandlePlaceBidAsync();
                    break;
                case "close-auction":
                case "5":
                    await HandleCloseAuctionAsync();
                    break;
                case "show-auctions":
                case "6":
                    await HandleShowAuctionsAsync();
                    break;
                case "show-types":
                case "7":
                    await HandleShowTypesAsync();
                    break;
                case "help":
                case "8":
                    ShowHelp();
                    break;
                default:
                    System.Console.WriteLine($"Unknown command: {commandName}. Type 'help' for available commands.");
                    break;
            }
        }

        private async Task HandleAddVehicleAsync()
        {
            System.Console.WriteLine("=== Add New Vehicle ===");
            
            var id = PromptForInput("Enter Vehicle ID: ");
            var vin = PromptForInput("Enter VIN: ");
            var type = PromptForInput("Enter Vehicle Type (Sedan/SUV/Truck/Hatchback): ");
            var manufacturer = PromptForInput("Enter Manufacturer: ");
            var model = PromptForInput("Enter Model: ");
            
            if (!int.TryParse(PromptForInput("Enter Year: "), out var year))
            {
                System.Console.WriteLine("Invalid year format.");
                return;
            }

            if (!decimal.TryParse(PromptForInput("Enter Starting Bid Amount: "), out var amount))
            {
                System.Console.WriteLine("Invalid amount format.");
                return;
            }

            var currency = PromptForInput("Enter Currency (default: USD): ");
            if (string.IsNullOrWhiteSpace(currency)) currency = "USD";

            var additionalParams = new Dictionary<string, object>();

            switch (type.ToLowerInvariant())
            {
                case "sedan":
                case "hatchback":
                    if (int.TryParse(PromptForInput("Enter Number of Doors: "), out var doors))
                        additionalParams["NumberOfDoors"] = doors;
                    else
                    {
                        System.Console.WriteLine("Invalid number of doors.");
                        return;
                    }
                    break;
                case "suv":
                    if (int.TryParse(PromptForInput("Enter Number of Seats: "), out var seats))
                        additionalParams["NumberOfSeats"] = seats;
                    else
                    {
                        System.Console.WriteLine("Invalid number of seats.");
                        return;
                    }
                    break;
                case "truck":
                    if (decimal.TryParse(PromptForInput("Enter Load Capacity (kg): "), out var capacity))
                        additionalParams["LoadCapacity"] = capacity;
                    else
                    {
                        System.Console.WriteLine("Invalid load capacity.");
                        return;
                    }
                    break;
                default:
                    System.Console.WriteLine("Unsupported vehicle type.");
                    return;
            }

            var request = new CreateVehicleRequest(id, vin, type, manufacturer, model, year, amount, currency, additionalParams);
            var result = await _auctionService.AddVehicleAsync(request);
            
            System.Console.WriteLine($"Vehicle added successfully: {result.Id} - {result.Manufacturer} {result.Model}");
        }

        private async Task HandleSearchAsync()
        {
            System.Console.WriteLine("=== Search Vehicles ===");
            
            var type = PromptForInput("Enter Vehicle Type (optional): ");
            var manufacturer = PromptForInput("Enter Manufacturer (optional): ");
            var model = PromptForInput("Enter Model (optional): ");
            
            int? year = null;
            var yearInput = PromptForInput("Enter Year (optional): ");
            if (!string.IsNullOrWhiteSpace(yearInput) && int.TryParse(yearInput, out var yearValue))
                year = yearValue;

            var request = new VehicleSearchRequest(
                string.IsNullOrWhiteSpace(type) ? null : type,
                string.IsNullOrWhiteSpace(manufacturer) ? null : manufacturer,
                string.IsNullOrWhiteSpace(model) ? null : model,
                year
            );

            var results = await _auctionService.SearchVehiclesAsync(request);
            
            System.Console.WriteLine($"\nSearch Results ({results.TotalCount} found):");
            System.Console.WriteLine($"Criteria: {results.SearchDescription}");
            System.Console.WriteLine();

            if (results.TotalCount == 0)
            {
                System.Console.WriteLine("No vehicles found matching the criteria.");
                return;
            }

            foreach (var vehicle in results.Vehicles)
            {
                System.Console.WriteLine($"ID: {vehicle.Id}");
                System.Console.WriteLine($"Type: {vehicle.Type}");
                System.Console.WriteLine($"Vehicle: {vehicle.Manufacturer} {vehicle.Model} ({vehicle.Year})");
                System.Console.WriteLine($"Starting Bid: {vehicle.StartingBidAmount:C} {vehicle.StartingBidCurrency}");
                System.Console.WriteLine($"Created: {vehicle.CreatedAt:yyyy-MM-dd HH:mm}");
                System.Console.WriteLine();
            }
        }

        private async Task HandleStartAuctionAsync()
        {
            System.Console.WriteLine("=== Start Auction ===");
            
            var vehicleId = PromptForInput("Enter Vehicle ID: ");
            
            var request = new StartAuctionRequest(vehicleId);
            var result = await _auctionService.StartAuctionAsync(request);
            
            System.Console.WriteLine($"Auction started for vehicle {result.VehicleId}");
            System.Console.WriteLine($"Starting bid: {result.CurrentHighestBidAmount:C} {result.CurrentHighestBidCurrency}");
            System.Console.WriteLine($"Started at: {result.StartTime:yyyy-MM-dd HH:mm:ss}");
        }

        private async Task HandlePlaceBidAsync()
        {
            System.Console.WriteLine("=== Place Bid ===");
            
            var vehicleId = PromptForInput("Enter Vehicle ID: ");
            var bidder = PromptForInput("Enter Bidder Name: ");
            
            if (!decimal.TryParse(PromptForInput("Enter Bid Amount: "), out var amount))
            {
                System.Console.WriteLine("Invalid amount format.");
                return;
            }

            var currency = PromptForInput("Enter Currency (default: USD): ");
            if (string.IsNullOrWhiteSpace(currency)) currency = "USD";

            var request = new PlaceBidRequest(vehicleId, bidder, amount, currency);
            var result = await _auctionService.PlaceBidAsync(request);
            
            System.Console.WriteLine($"Bid placed successfully!");
            System.Console.WriteLine($"New highest bid: {result.CurrentHighestBidAmount:C} {result.CurrentHighestBidCurrency} by {result.CurrentHighestBidder}");
            System.Console.WriteLine($"Total bids: {result.TotalBids}");
        }

        private async Task HandleCloseAuctionAsync()
        {
            System.Console.WriteLine("=== Close Auction ===");
            
            var vehicleId = PromptForInput("Enter Vehicle ID: ");
            
            var request = new CloseAuctionRequest(vehicleId);
            var result = await _auctionService.CloseAuctionAsync(request);
            
            System.Console.WriteLine($"Auction closed for vehicle {result.VehicleId}");
            
            if (result.TotalBids > 0)
            {
                System.Console.WriteLine($"Winning bid: {result.CurrentHighestBidAmount:C} {result.CurrentHighestBidCurrency} by {result.CurrentHighestBidder}");
                System.Console.WriteLine($"Total bids: {result.TotalBids}");
            }
            else
            {
                System.Console.WriteLine("No bids were placed on this auction.");
            }
            
            System.Console.WriteLine($"Duration: {result.StartTime:yyyy-MM-dd HH:mm} - {result.EndTime:yyyy-MM-dd HH:mm}");
        }

        private async Task HandleShowAuctionsAsync()
        {
            System.Console.WriteLine("=== Active Auctions ===");
            
            var auctions = await _auctionService.GetAllActiveAuctionsAsync();
            var auctionList = auctions.ToList();
            
            if (!auctionList.Any())
            {
                System.Console.WriteLine("No active auctions found.");
                return;
            }

            foreach (var auction in auctionList)
            {
                System.Console.WriteLine($"Vehicle ID: {auction.VehicleId}");
                System.Console.WriteLine($"Current Highest Bid: {auction.CurrentHighestBidAmount:C} {auction.CurrentHighestBidCurrency}");
                
                if (!string.IsNullOrEmpty(auction.CurrentHighestBidder))
                    System.Console.WriteLine($"Current Highest Bidder: {auction.CurrentHighestBidder}");
                else
                    System.Console.WriteLine("No bids yet");
                
                System.Console.WriteLine($"Started: {auction.StartTime:yyyy-MM-dd HH:mm:ss}");
                System.Console.WriteLine($"Total Bids: {auction.TotalBids}");
                
                if (auction.RecentBids.Any())
                {
                    System.Console.WriteLine("Recent Bids:");
                    foreach (var bid in auction.RecentBids.Take(3))
                    {
                        System.Console.WriteLine($"  {bid.Bidder}: {bid.Amount:C} {bid.Currency} at {bid.PlacedAt:HH:mm:ss}");
                    }
                }
                
                System.Console.WriteLine();
            }
        }

        private async Task HandleShowTypesAsync()
        {
            System.Console.WriteLine("=== Supported Vehicle Types ===");
            
            var types = await _auctionService.GetSupportedVehicleTypesAsync();
            
            foreach (var type in types)
            {
                System.Console.WriteLine($"- {type}");
            }
        }

        private static void ShowHelp()
        {
            System.Console.WriteLine("=== Command Help ===");
            System.Console.WriteLine();
            System.Console.WriteLine("add-vehicle    : Add a new vehicle to the inventory");
            System.Console.WriteLine("                 Prompts for vehicle details based on type");
            System.Console.WriteLine();
            System.Console.WriteLine("search         : Search for vehicles by type, manufacturer, model, or year");
            System.Console.WriteLine("                 All criteria are optional - leave blank to skip");
            System.Console.WriteLine();
            System.Console.WriteLine("start-auction  : Start an auction for a specific vehicle");
            System.Console.WriteLine("                 Vehicle must exist and not have an active auction");
            System.Console.WriteLine();
            System.Console.WriteLine("place-bid      : Place a bid on an active auction");
            System.Console.WriteLine("                 Bid must be higher than current highest bid");
            System.Console.WriteLine();
            System.Console.WriteLine("close-auction  : Close an active auction");
            System.Console.WriteLine("                 Shows final results and winning bid");
            System.Console.WriteLine();
            System.Console.WriteLine("show-auctions  : Display all currently active auctions");
            System.Console.WriteLine("                 Shows current bids and recent activity");
            System.Console.WriteLine();
            System.Console.WriteLine("show-types     : List all supported vehicle types");
            System.Console.WriteLine();
            System.Console.WriteLine("Vehicle Types and Required Parameters:");
            System.Console.WriteLine("- Sedan      : NumberOfDoors (2-5)");
            System.Console.WriteLine("- Hatchback  : NumberOfDoors (2-5)");
            System.Console.WriteLine("- SUV        : NumberOfSeats (2-9)");
            System.Console.WriteLine("- Truck      : LoadCapacity in kg (0.1-100000)");
        }

        private static string PromptForInput(string prompt)
        {
            System.Console.Write(prompt);
            return System.Console.ReadLine()?.Trim() ?? string.Empty;
        }
    }
}