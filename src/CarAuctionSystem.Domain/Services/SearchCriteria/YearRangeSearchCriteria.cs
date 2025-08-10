using System;
using CarAuctionSystem.Domain.Entities;

namespace CarAuctionSystem.Domain.Services.SearchCriteria
{
    public class YearRangeSearchCriteria : SearchCriteria
    {
        private readonly int _minYear;
        private readonly int _maxYear;

        public YearRangeSearchCriteria(int minYear, int maxYear)
        {
            if (minYear > maxYear)
                throw new ArgumentException("Min year cannot be greater than max year");
            
            _minYear = minYear;
            _maxYear = maxYear;
        }

        public override bool Matches(Vehicle vehicle) => 
            vehicle.Year >= _minYear && vehicle.Year <= _maxYear;

        public override string GetDescription() => $"Year Range: {_minYear}-{_maxYear}";
    }
}