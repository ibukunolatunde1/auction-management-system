using System.Collections.Generic;
using CarAuctionSystem.Domain.ValueObjects;

namespace CarAuctionSystem.Domain.Entities
{
    public class Truck : Vehicle
    {
        public decimal LoadCapacity { get; }

        public Truck(VehicleId id, string vin, string manufacturer, string model, int year, Money startingBid, decimal loadCapacity)
            : base(id, vin, manufacturer, model, year, startingBid)
        {
            LoadCapacity = ValidatePositiveDecimal(loadCapacity, nameof(loadCapacity), 0.1m, 100000m);
        }

        public override string GetVehicleType() => "Truck";

        public override Dictionary<string, object> GetSearchableAttributes() => new()
        {
            ["Type"] = GetVehicleType(),
            ["Manufacturer"] = Manufacturer,
            ["Model"] = Model,
            ["Year"] = Year,
            ["LoadCapacity"] = LoadCapacity
        };
    }
}