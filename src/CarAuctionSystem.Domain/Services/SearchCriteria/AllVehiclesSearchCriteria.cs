using CarAuctionSystem.Domain.Entities;

namespace CarAuctionSystem.Domain.Services.SearchCriteria
{
    public class AllVehiclesSearchCriteria : SearchCriteria
    {
        public override bool Matches(Vehicle vehicle) => true;

        public override string GetDescription() => "All Vehicles";
    }
}