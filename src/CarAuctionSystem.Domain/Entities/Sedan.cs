using System.Collections.Generic;
using CarAuctionSystem.Domain.ValueObjects;

namespace CarAuctionSystem.Domain.Entities
{
    public class Sedan : Vehicle
    {
        public int NumberOfDoors { get; }

        public Sedan(VehicleId id, string vin, string manufacturer, string model, int year, Money startingBid, int numberOfDoors)
            : base(id, vin, manufacturer, model, year, startingBid)
        {
            NumberOfDoors = ValidatePositiveInteger(numberOfDoors, nameof(numberOfDoors), 2, 5);
        }

        public override string GetVehicleType() => "Sedan";

        public override Dictionary<string, object> GetSearchableAttributes() => new()
        {
            ["Type"] = GetVehicleType(),
            ["Manufacturer"] = Manufacturer,
            ["Model"] = Model,
            ["Year"] = Year,
            ["NumberOfDoors"] = NumberOfDoors
        };
    }
}