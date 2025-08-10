using System;

namespace CarAuctionSystem.Domain.ValueObjects
{
    public record VehicleId
    {
        public string Value { get; }

        public VehicleId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Vehicle ID cannot be empty", nameof(value));
            
            Value = value.Trim();
        }

        public static implicit operator string(VehicleId id) => id.Value;
        public static implicit operator VehicleId(string value) => new(value);
        
        public override string ToString() => Value;
    }
}