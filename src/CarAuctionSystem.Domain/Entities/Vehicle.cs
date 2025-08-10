using System;
using System.Collections.Generic;
using CarAuctionSystem.Domain.ValueObjects;
using CarAuctionSystem.Domain.Exceptions;

namespace CarAuctionSystem.Domain.Entities
{
    public abstract class Vehicle
    {
        public VehicleId Id { get; }
        public string Manufacturer { get; }

        public string Vin { get; }
        public string Model { get; }
        public int Year { get; }
        public Money StartingBid { get; }
        public DateTime CreatedAt { get; }

        protected Vehicle(VehicleId id, string vin, string manufacturer, string model, int year, Money startingBid)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));

            Vin = ValidateString(vin, nameof(vin));
            Manufacturer = ValidateString(manufacturer, nameof(manufacturer));
            Model = ValidateString(model, nameof(model));
            Year = ValidateYear(year);
            StartingBid = startingBid ?? throw new ArgumentNullException(nameof(startingBid));
            CreatedAt = DateTime.UtcNow;

            if (startingBid.Amount <= 0)
                throw new InvalidVehicleDataException("Starting bid must be positive");
        }

        private static string ValidateString(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} cannot be empty", paramName);
            return value.Trim();
        }

        private static int ValidateYear(int year)
        {
            var currentYear = DateTime.Now.Year;
            if (year < 1900 || year > currentYear + 1)
                throw new ArgumentOutOfRangeException(nameof(year), 
                    $"Year must be between 1900 and {currentYear + 1}");
            return year;
        }

        public abstract string GetVehicleType();
        public abstract Dictionary<string, object> GetSearchableAttributes();

        protected static int ValidatePositiveInteger(int value, string paramName, int min = 1, int max = int.MaxValue)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(paramName, 
                    $"{paramName} must be between {min} and {max}");
            return value;
        }

        protected static decimal ValidatePositiveDecimal(decimal value, string paramName, decimal min = 0.1m, decimal max = decimal.MaxValue)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(paramName, 
                    $"{paramName} must be between {min} and {max}");
            return value;
        }
    }
}