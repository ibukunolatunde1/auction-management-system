using System;
using CarAuctionSystem.Domain.Entities;

namespace CarAuctionSystem.Domain.Services.SearchCriteria
{
    public class ManufacturerSearchCriteria : SearchCriteria
    {
        private readonly string _manufacturer;

        public ManufacturerSearchCriteria(string manufacturer)
        {
            _manufacturer = manufacturer?.Trim() ?? throw new ArgumentNullException(nameof(manufacturer));
        }

        public override bool Matches(Vehicle vehicle) => 
            string.Equals(vehicle.Manufacturer, _manufacturer, StringComparison.OrdinalIgnoreCase);

        public override string GetDescription() => $"Manufacturer: {_manufacturer}";
    }
}