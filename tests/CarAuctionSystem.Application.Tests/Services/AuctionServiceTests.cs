using System;
using System.Threading;
using System.Threading.Tasks;
using CarAuctionSystem.Application.DTOs;
using CarAuctionSystem.Application.Factories;
using CarAuctionSystem.Application.Interfaces.Repositories;
using CarAuctionSystem.Application.Services;
using CarAuctionSystem.Domain.Entities;
using CarAuctionSystem.Domain.Exceptions;
using CarAuctionSystem.Domain.ValueObjects;
using Moq;
using Xunit;

namespace CarAuctionSystem.Application.Tests.Services
{
    public class AuctionServiceTests
    {
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
        private readonly Mock<IVehicleFactory> _vehicleFactoryMock;
        private readonly AuctionService _auctionService;

        public AuctionServiceTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            _vehicleFactoryMock = new Mock<IVehicleFactory>();
            _auctionService = new AuctionService(
                _vehicleRepositoryMock.Object,
                _auctionRepositoryMock.Object,
                _vehicleFactoryMock.Object);
        }

        [Fact]
        public async Task AddVehicleAsync_WithNewVehicle_AddsVehicleSuccessfully()
        {
            // Arrange
            var request = new CreateVehicleRequest(
                "test-id",
                "sedan",
                "1HGCM82633A123456",
                "Test",
                "TestModel",
                2023,
                1000m,
                "USD",
                new Dictionary<string, object>()
            );

            var vehicleId = new VehicleId(request.Id);
            _vehicleRepositoryMock.Setup(x => x.ExistsAsync(vehicleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var vehicle = new TestVehicle(
                vehicleId,
                request.Vin,
                request.Manufacturer,
                request.Model,
                request.Year,
                new Money(request.StartingBidAmount, request.StartingBidCurrency)
            );

            _vehicleFactoryMock.Setup(x => x.CreateVehicle(
                    request.Type,
                    request.Vin,
                    It.IsAny<VehicleId>(),
                    request.Manufacturer,
                    request.Model,
                    request.Year,
                    It.IsAny<Money>(),
                    request.AdditionalAttributes))
                .Returns(vehicle);

            // Act
            var result = await _auctionService.AddVehicleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Id, result.Id);
            Assert.Equal(request.Manufacturer, result.Manufacturer);
            Assert.Equal(request.Model, result.Model);
            Assert.Equal(request.Year, result.Year);

            _vehicleRepositoryMock.Verify(x => x.AddAsync(vehicle, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task StartAuctionAsync_WithValidVehicle_StartsAuctionSuccessfully()
        {
            // Arrange
            var vehicleId = new VehicleId("test-id");
            var vehicle = new TestVehicle(
                vehicleId,
                "1HGCM82633A123456",
                "Test",
                "TestModel",
                2023,
                new Money(1000m, "USD")
            );

            var request = new StartAuctionRequest(vehicleId.Value);

            _vehicleRepositoryMock.Setup(x => x.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vehicle);

            _auctionRepositoryMock.Setup(x => x.GetActiveAuctionAsync(vehicleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Auction?)null);

            // Act
            var result = await _auctionService.StartAuctionAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vehicleId.Value, result.VehicleId);
            Assert.True(result.IsActive);
            Assert.Equal(0, result.TotalBids);

            _auctionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Auction>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task StartAuctionAsync_WithExistingActiveAuction_ThrowsException()
        {
            // Arrange
            var vehicleId = new VehicleId("test-id");
            var vehicle = new TestVehicle(
                vehicleId,
                "1HGCM82633A123456",
                "Test",
                "TestModel",
                2023,
                new Money(1000m, "USD")
            );

            var request = new StartAuctionRequest(vehicleId.Value);
            var existingAuction = new Auction(vehicleId, vehicle.StartingBid);

            _vehicleRepositoryMock.Setup(x => x.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vehicle);

            _auctionRepositoryMock.Setup(x => x.GetActiveAuctionAsync(vehicleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAuction);

            // Act & Assert
            await Assert.ThrowsAsync<AuctionAlreadyActiveException>(() => 
                _auctionService.StartAuctionAsync(request));
        }

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
    }
}
