using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CarAuctionSystem.Domain.Entities;
using CarAuctionSystem.Domain.ValueObjects;
using CarAuctionSystem.Infrastructure.Repositories;
using Xunit;

namespace CarAuctionSystem.Infrastructure.Tests.Repositories
{
    public class InMemoryAuctionRepositoryTests
    {
        private readonly InMemoryAuctionRepository _repository;
        private readonly VehicleId _vehicleId;
        private readonly Money _startingBid;

        public InMemoryAuctionRepositoryTests()
        {
            _repository = new InMemoryAuctionRepository();
            _vehicleId = new VehicleId(Guid.NewGuid().ToString());
            _startingBid = new Money(1000m, "USD");
        }

        [Fact]
        public async Task AddAsync_WithValidAuction_AddsSuccessfully()
        {
            // Arrange
            var auction = new Auction(_vehicleId, _startingBid);

            // Act
            await _repository.AddAsync(auction);
            var result = await _repository.GetActiveAuctionAsync(_vehicleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_vehicleId, result.VehicleId);
            Assert.Equal(_startingBid, result.CurrentHighestBid);
        }

        [Fact]
        public async Task AddAsync_WithDuplicateActiveAuction_ThrowsException()
        {
            // Arrange
            var auction1 = new Auction(_vehicleId, _startingBid);
            var auction2 = new Auction(_vehicleId, _startingBid);

            await _repository.AddAsync(auction1);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _repository.AddAsync(auction2));
        }

        [Fact]
        public async Task GetActiveAuctionAsync_WithExistingAuction_ReturnsAuction()
        {
            // Arrange
            var auction = new Auction(_vehicleId, _startingBid);
            await _repository.AddAsync(auction);

            // Act
            var result = await _repository.GetActiveAuctionAsync(_vehicleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_vehicleId, result.VehicleId);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task GetActiveAuctionAsync_WithClosedAuction_ReturnsNull()
        {
            // Arrange
            var auction = new Auction(_vehicleId, _startingBid);
            await _repository.AddAsync(auction);
            auction.Close();

            // Act
            var result = await _repository.GetActiveAuctionAsync(_vehicleId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllActiveAuctionsAsync_WithMultipleAuctions_ReturnsOnlyActiveAuctions()
        {
            // Arrange
            var auction1 = new Auction(new VehicleId(Guid.NewGuid().ToString()), _startingBid);
            var auction2 = new Auction(new VehicleId(Guid.NewGuid().ToString()), _startingBid);
            var auction3 = new Auction(new VehicleId(Guid.NewGuid().ToString()), _startingBid);

            await _repository.AddAsync(auction1);
            await _repository.AddAsync(auction2);
            await _repository.AddAsync(auction3);

            auction2.Close();

            // Act
            var results = (await _repository.GetAllActiveAuctionsAsync()).ToList();

            // Assert
            Assert.Equal(2, results.Count);
            Assert.All(results, r => Assert.True(r.IsActive));
            Assert.DoesNotContain(results, r => r.VehicleId == auction2.VehicleId);
        }

        [Fact]
        public async Task GetActiveAuctionCountAsync_ReturnsCorrectCount()
        {
            // Arrange
            var auction1 = new Auction(new VehicleId(Guid.NewGuid().ToString()), _startingBid);
            var auction2 = new Auction(new VehicleId(Guid.NewGuid().ToString()), _startingBid);

            await _repository.AddAsync(auction1);
            await _repository.AddAsync(auction2);
            auction1.Close();

            // Act
            var count = await _repository.GetActiveAuctionCountAsync();

            // Assert
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task ClearAsync_RemovesAllAuctions()
        {
            // Arrange
            var auction1 = new Auction(new VehicleId(Guid.NewGuid().ToString()), _startingBid);
            var auction2 = new Auction(new VehicleId(Guid.NewGuid().ToString()), _startingBid);

            await _repository.AddAsync(auction1);
            await _repository.AddAsync(auction2);

            // Act
            await _repository.ClearAsync();
            var activeAuctions = await _repository.GetAllActiveAuctionsAsync();

            // Assert
            Assert.Empty(activeAuctions);
        }
    }
}
