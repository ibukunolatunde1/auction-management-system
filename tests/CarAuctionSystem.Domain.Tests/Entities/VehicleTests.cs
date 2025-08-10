using System;
using CarAuctionSystem.Domain.Entities;
using CarAuctionSystem.Domain.Exceptions;
using CarAuctionSystem.Domain.ValueObjects;
using Xunit;

namespace CarAuctionSystem.Domain.Tests.Entities
{
    public class VehicleTests
    {
        private class TestVehicle : Vehicle
        {
            public TestVehicle(VehicleId id, string vin, string manufacturer, string model, int year, Money startingBid)
                : base(id, vin, manufacturer, model, year, startingBid)
            {
            }

            public override string GetVehicleType() => "Test";

            public override Dictionary<string, object> GetSearchableAttributes()
            {
                return new Dictionary<string, object>
                {
                    { "type", GetVehicleType() }
                };
            }
        }

        [Fact]
        public void Constructor_WithValidData_CreatesVehicle()
        {
            // Arrange
            var id = new VehicleId(Guid.NewGuid().ToString());
            var vin = "1HGCM82633A123456";
            var manufacturer = "Test";
            var model = "TestModel";
            var year = DateTime.Now.Year;
            var startingBid = new Money(1000m, "USD");

            // Act
            var vehicle = new TestVehicle(id, vin, manufacturer, model, year, startingBid);

            // Assert
            Assert.Equal(id, vehicle.Id);
            Assert.Equal(vin, vehicle.Vin);
            Assert.Equal(manufacturer, vehicle.Manufacturer);
            Assert.Equal(model, vehicle.Model);
            Assert.Equal(year, vehicle.Year);
            Assert.Equal(startingBid, vehicle.StartingBid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_WithInvalidVin_ThrowsArgumentException(string vin)
        {
            // Arrange
            var id = new VehicleId(Guid.NewGuid().ToString());
            var manufacturer = "Test";
            var model = "TestModel";
            var year = DateTime.Now.Year;
            var startingBid = new Money(1000m, "USD");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new TestVehicle(id, vin, manufacturer, model, year, startingBid));
        }

        [Theory]
        [InlineData(1899)]  // Too old
        [InlineData(2050)]  // Too far in the future
        public void Constructor_WithInvalidYear_ThrowsArgumentOutOfRangeException(int year)
        {
            // Arrange
            var id = new VehicleId(Guid.NewGuid().ToString());
            var vin = "1HGCM82633A123456";
            var manufacturer = "Test";
            var model = "TestModel";
            var startingBid = new Money(1000m, "USD");

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new TestVehicle(id, vin, manufacturer, model, year, startingBid));
        }

        [Fact]
        public void Constructor_WithZeroStartingBid_ThrowsInvalidVehicleDataException()
        {
            // Arrange
            var id = new VehicleId(Guid.NewGuid().ToString());
            var vin = "1HGCM82633A123456";
            var manufacturer = "Test";
            var model = "TestModel";
            var year = DateTime.Now.Year;
            var startingBid = new Money(0m, "USD");

            // Act & Assert
            Assert.Throws<InvalidVehicleDataException>(() => 
                new TestVehicle(id, vin, manufacturer, model, year, startingBid));
        }
    }
}
