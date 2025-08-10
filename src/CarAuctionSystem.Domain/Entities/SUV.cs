using System.Collections.Generic;
using CarAuctionSystem.Domain.ValueObjects;

namespace CarAuctionSystem.Domain.Entities
{
    public class SUV : Vehicle
    {
        public int NumberOfSeats { get; }

        public SUV(VehicleId id, string vin, string manufacturer, string model, int year, Money startingBid, int numberOfSeats)
            : base(id, vin, manufacturer, model, year, startingBid)
        {
            NumberOfSeats = ValidatePositiveInteger(numberOfSeats, nameof(numberOfSeats), 2, 9);
        }

        public override string GetVehicleType() => "SUV";

        public override Dictionary<string, object> GetSearchableAttributes() => new()
        {
            ["Type"] = GetVehicleType(),
            ["Manufacturer"] = Manufacturer,
            ["Model"] = Model,
            ["Year"] = Year,
            ["NumberOfSeats"] = NumberOfSeats
        };
    }
}