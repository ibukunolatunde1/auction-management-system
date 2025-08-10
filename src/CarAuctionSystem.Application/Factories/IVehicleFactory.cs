using System.Collections.Generic;
using CarAuctionSystem.Domain.Entities;
using CarAuctionSystem.Domain.ValueObjects;

namespace CarAuctionSystem.Application.Factories
{
    public interface IVehicleFactory
    {
        Vehicle CreateVehicle(
            string type, 
            string vin,
            VehicleId id, 
            string manufacturer, 
            string model, 
            int year, 
            Money startingBid, 
            Dictionary<string, object> additionalParams
        );
        
        IEnumerable<string> GetSupportedVehicleTypes();
        Dictionary<string, string> GetRequiredParametersForType(string type);
    }
}