using CarAuctionSystem.Domain.Entities;

namespace CarAuctionSystem.Domain.Services.SearchCriteria
{
    public class YearSearchCriteria : SearchCriteria
    {
        private readonly int _year;

        public YearSearchCriteria(int year)
        {
            _year = year;
        }

        public override bool Matches(Vehicle vehicle) => vehicle.Year == _year;

        public override string GetDescription() => $"Year: {_year}";
    }
}