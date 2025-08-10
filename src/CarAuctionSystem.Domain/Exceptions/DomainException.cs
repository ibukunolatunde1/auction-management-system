using System;

namespace CarAuctionSystem.Domain.Exceptions
{
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
        protected DomainException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class VehicleAlreadyExistsException : DomainException
    {
        public VehicleAlreadyExistsException(string vehicleId) 
            : base($"Vehicle with ID '{vehicleId}' already exists") { }
    }

    public class VehicleNotFoundException : DomainException
    {
        public VehicleNotFoundException(string vehicleId) 
            : base($"Vehicle with ID '{vehicleId}' not found") { }
    }

    public class AuctionAlreadyActiveException : DomainException
    {
        public AuctionAlreadyActiveException(string vehicleId) 
            : base($"Auction for vehicle '{vehicleId}' is already active") { }
    }

    public class AuctionNotFoundException : DomainException
    {
        public AuctionNotFoundException(string vehicleId) 
            : base($"No active auction found for vehicle '{vehicleId}'") { }
    }

    public class InvalidBidException : DomainException
    {
        public InvalidBidException(string message) : base(message) { }
    }

    public class InvalidVehicleDataException : DomainException
    {
        public InvalidVehicleDataException(string message) : base(message) { }
    }
}