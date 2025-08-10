using System;
using CarAuctionSystem.Domain.Entities;

namespace CarAuctionSystem.Domain.Services.SearchCriteria
{
    public class ModelSearchCriteria : SearchCriteria
    {
        private readonly string _model;

        public ModelSearchCriteria(string model)
        {
            _model = model?.Trim() ?? throw new ArgumentNullException(nameof(model));
        }

        public override bool Matches(Vehicle vehicle) => 
            string.Equals(vehicle.Model, _model, StringComparison.OrdinalIgnoreCase);

        public override string GetDescription() => $"Model: {_model}";
    }
}