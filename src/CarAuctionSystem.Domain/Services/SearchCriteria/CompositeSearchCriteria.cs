using System;
using System.Collections.Generic;
using System.Linq;
using CarAuctionSystem.Domain.Entities;

namespace CarAuctionSystem.Domain.Services.SearchCriteria
{
    public class CompositeSearchCriteria : SearchCriteria
    {
        private readonly IReadOnlyList<SearchCriteria> _criteria;

        public CompositeSearchCriteria(params SearchCriteria[] criteria)
        {
            if (criteria == null || criteria.Length == 0)
                throw new ArgumentException("At least one criteria must be provided", nameof(criteria));
            
            _criteria = criteria.ToList().AsReadOnly();
        }

        public override bool Matches(Vehicle vehicle) => 
            _criteria.All(criteria => criteria.Matches(vehicle));

        public override string GetDescription() => 
            $"Combined: [{string.Join(" AND ", _criteria.Select(c => c.GetDescription()))}]";
    }
}