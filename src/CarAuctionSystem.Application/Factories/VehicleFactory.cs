using System;
using System.Collections.Generic;
using System.Globalization;
using CarAuctionSystem.Domain.Entities;
using CarAuctionSystem.Domain.ValueObjects;

namespace CarAuctionSystem.Application.Factories
{
    public class VehicleFactory : IVehicleFactory
    {
        private static readonly Dictionary<string, Dictionary<string, string>> _typeParameters = new()
        {
            ["sedan"] = new() { ["NumberOfDoors"] = "int" },
            ["hatchback"] = new() { ["NumberOfDoors"] = "int" },
            ["suv"] = new() { ["NumberOfSeats"] = "int" },
            ["truck"] = new() { ["LoadCapacity"] = "decimal" }
        };

        public Vehicle CreateVehicle(
            string type, 
            VehicleId id, 
            string vin,
            string manufacturer, 
            string model, 
            int year, 
            Money startingBid, 
            Dictionary<string, object> additionalParams)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Vehicle type cannot be empty", nameof(type));
            
            if (additionalParams == null)
                throw new ArgumentNullException(nameof(additionalParams));

            return type.ToLowerInvariant() switch
            {
                "sedan" => CreateSedan(id, vin, manufacturer, model, year, startingBid, additionalParams),
                "hatchback" => CreateHatchback(id, vin, manufacturer, model, year, startingBid, additionalParams),
                "suv" => CreateSUV(id, vin, manufacturer, model, year, startingBid, additionalParams),
                "truck" => CreateTruck(id, vin, manufacturer, model, year, startingBid, additionalParams),
                _ => throw new ArgumentException($"Unknown vehicle type: {type}", nameof(type))
            };
        }

        public IEnumerable<string> GetSupportedVehicleTypes()
        {
            return _typeParameters.Keys;
        }

        public Dictionary<string, string> GetRequiredParametersForType(string type)
        {
            var normalizedType = type?.ToLowerInvariant();
            if (normalizedType != null && _typeParameters.TryGetValue(normalizedType, out var parameters))
            {
                return new Dictionary<string, string>(parameters);
            }
            
            throw new ArgumentException($"Unknown vehicle type: {type}", nameof(type));
        }

        private static Sedan CreateSedan(VehicleId id, string vin, string manufacturer, string model, int year, Money startingBid, Dictionary<string, object> additionalParams)
        {
            var numberOfDoors = GetRequiredParameter<int>(additionalParams, "NumberOfDoors");
            return new Sedan(id, vin, manufacturer, model, year, startingBid, numberOfDoors);
        }

        private static Hatchback CreateHatchback(VehicleId id, string vin, string manufacturer, string model, int year, Money startingBid, Dictionary<string, object> additionalParams)
        {
            var numberOfDoors = GetRequiredParameter<int>(additionalParams, "NumberOfDoors");
            return new Hatchback(id, vin, manufacturer, model, year, startingBid, numberOfDoors);
        }

        private static SUV CreateSUV(VehicleId id, string vin, string manufacturer, string model, int year, Money startingBid, Dictionary<string, object> additionalParams)
        {
            var numberOfSeats = GetRequiredParameter<int>(additionalParams, "NumberOfSeats");
            return new SUV(id, vin, manufacturer, model, year, startingBid, numberOfSeats);
        }

        private static Truck CreateTruck(VehicleId id, string vin, string manufacturer, string model, int year, Money startingBid, Dictionary<string, object> additionalParams)
        {
            var loadCapacity = GetRequiredParameter<decimal>(additionalParams, "LoadCapacity");
            return new Truck(id, vin, manufacturer, model, year, startingBid, loadCapacity);
        }

        private static T GetRequiredParameter<T>(Dictionary<string, object> parameters, string key)
        {
            if (!parameters.TryGetValue(key, out var value))
                throw new ArgumentException($"Required parameter '{key}' is missing", nameof(parameters));

            try
            {
                if (typeof(T) == typeof(int) && value is long longValue)
                {
                    return (T)(object)(int)longValue;
                }
                
                return (T)Convert.ChangeType(value.ToString(), typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Parameter '{key}' cannot be converted to {typeof(T).Name}. Current value type: {value?.GetType().Name}", nameof(parameters), ex);
            }
        }

        public Vehicle CreateVehicle(string type, string vin, VehicleId id, string manufacturer, string model, int year, Money startingBid, Dictionary<string, object> additionalParams)
        {
            switch (type.ToLowerInvariant())
            {
                case "sedan":
                    return CreateSedan(id, vin, manufacturer, model, year, startingBid, additionalParams);
                case "hatchback":
                    return CreateHatchback(id, vin, manufacturer, model, year, startingBid, additionalParams);
                case "suv":
                    return CreateSUV(id, vin, manufacturer, model, year, startingBid, additionalParams);
                case "truck":
                    return CreateTruck(id, vin, manufacturer, model, year, startingBid, additionalParams);
                default:
                    throw new ArgumentException($"Unknown vehicle type: {type}", nameof(type));
            }
        }
    }
}