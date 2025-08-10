using CarAuctionSystem.Domain.Entities;

namespace CarAuctionSystem.Domain.Services.SearchCriteria
{
    public abstract class SearchCriteria
    {
        public abstract bool Matches(Vehicle vehicle);
        
        public virtual string GetDescription() => GetType().Name;
        
        public static SearchCriteria Combine(params SearchCriteria[] criteria)
        {
            return new CompositeSearchCriteria(criteria);
        }
    }
}