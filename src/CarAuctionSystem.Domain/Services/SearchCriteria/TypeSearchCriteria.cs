using System;
using CarAuctionSystem.Domain.Entities;

namespace CarAuctionSystem.Domain.Services.SearchCriteria
{
    public class TypeSearchCriteria : SearchCriteria
    {
        private readonly string _type;

        public TypeSearchCriteria(string type)
        {
            _type = type?.Trim() ?? throw new ArgumentNullException(nameof(type));
        }

        public override bool Matches(Vehicle vehicle) => 
            string.Equals(vehicle.GetVehicleType(), _type, StringComparison.OrdinalIgnoreCase);

        public override string GetDescription() => $"Type: {_type}";
    }
}