using System;
using System.Linq;
using CarAuctionSystem.Domain.Entities;
using CarAuctionSystem.Domain.Exceptions;
using CarAuctionSystem.Domain.ValueObjects;
using Xunit;

namespace CarAuctionSystem.Domain.Tests.Entities
{
    public class AuctionTests
    {
        private readonly VehicleId _vehicleId;
        private readonly Money _startingBid;

        public AuctionTests()
        {
            _vehicleId = new VehicleId(Guid.NewGuid().ToString());
            _startingBid = new Money(1000m, "USD");
        }

        [Fact]
        public void Constructor_WithValidData_CreatesAuction()
        {
            // Act
            var auction = new Auction(_vehicleId, _startingBid);

            // Assert
            Assert.Equal(_vehicleId, auction.VehicleId);
            Assert.Equal(_startingBid, auction.CurrentHighestBid);
            Assert.True(auction.IsActive);
            Assert.Null(auction.EndTime);
            Assert.Empty(auction.Bids);
        }

        [Fact]
        public void PlaceBid_WithHigherAmount_UpdatesCurrentHighestBid()
        {
            // Arrange
            var auction = new Auction(_vehicleId, _startingBid);
            var bidder = "TestBidder";
            var bidAmount = new Money(1500m, "USD");

            // Act
            auction.PlaceBid(bidder, bidAmount);

            // Assert
            Assert.Equal(bidAmount, auction.CurrentHighestBid);
            Assert.Equal(bidder, auction.CurrentHighestBidder);
            Assert.Single(auction.Bids);
            Assert.Equal(bidAmount, auction.Bids.First().Amount);
            Assert.Equal(bidder, auction.Bids.First().Bidder);
        }

        [Fact]
        public void PlaceBid_WithLowerAmount_ThrowsInvalidBidException()
        {
            // Arrange
            var auction = new Auction(_vehicleId, _startingBid);
            var bidder = "TestBidder";
            var lowerBidAmount = new Money(500m, "USD");

            // Act & Assert
            Assert.Throws<InvalidBidException>(() => 
                auction.PlaceBid(bidder, lowerBidAmount));
        }

        [Fact]
        public void PlaceBid_OnClosedAuction_ThrowsInvalidBidException()
        {
            // Arrange
            var auction = new Auction(_vehicleId, _startingBid);
            auction.Close();
            var bidder = "TestBidder";
            var bidAmount = new Money(1500m, "USD");

            // Act & Assert
            Assert.Throws<InvalidBidException>(() => 
                auction.PlaceBid(bidder, bidAmount));
        }

        [Fact]
        public void Close_OnActiveAuction_ClosesAuction()
        {
            // Arrange
            var auction = new Auction(_vehicleId, _startingBid);

            // Act
            auction.Close();

            // Assert
            Assert.False(auction.IsActive);
            Assert.NotNull(auction.EndTime);
        }

        [Fact]
        public void Close_OnClosedAuction_ThrowsInvalidOperationException()
        {
            // Arrange
            var auction = new Auction(_vehicleId, _startingBid);
            auction.Close();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => auction.Close());
        }

        [Fact]
        public void GetSummary_ReturnsCorrectSummary()
        {
            // Arrange
            var auction = new Auction(_vehicleId, _startingBid);
            var bidder = "TestBidder";
            var bidAmount = new Money(1500m, "USD");
            auction.PlaceBid(bidder, bidAmount);

            // Act
            var summary = auction.GetSummary();

            // Assert
            Assert.Equal(_vehicleId, summary.VehicleId);
            Assert.Equal(bidAmount, summary.CurrentHighestBid);
            Assert.Equal(bidder, summary.CurrentHighestBidder);
            Assert.Equal(1, summary.TotalBids);
            Assert.True(summary.IsActive);
        }
    }
}
