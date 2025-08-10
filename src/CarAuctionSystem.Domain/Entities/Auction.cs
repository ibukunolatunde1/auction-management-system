using System;
using System.Collections.Generic;
using System.Linq;
using CarAuctionSystem.Domain.ValueObjects;
using CarAuctionSystem.Domain.Exceptions;

namespace CarAuctionSystem.Domain.Entities
{
    public class Auction
    {
        private readonly List<Bid> _bids = new();

        public VehicleId VehicleId { get; }
        public DateTime StartTime { get; }
        public DateTime? EndTime { get; private set; }
        public Money CurrentHighestBid { get; private set; }
        public string? CurrentHighestBidder { get; private set; }
        public bool IsActive => !EndTime.HasValue;
        public IReadOnlyList<Bid> Bids => _bids.AsReadOnly();

        public Auction(VehicleId vehicleId, Money startingBid)
        {
            VehicleId = vehicleId ?? throw new ArgumentNullException(nameof(vehicleId));
            CurrentHighestBid = startingBid ?? throw new ArgumentNullException(nameof(startingBid));
            StartTime = DateTime.UtcNow;
        }

        public void PlaceBid(string bidder, Money amount)
        {
            ValidateBidder(bidder);
            ValidateAuctionActive();
            ValidateBidAmount(amount);

            var bid = new Bid(bidder, amount);
            _bids.Add(bid);
            CurrentHighestBid = amount;
            CurrentHighestBidder = bidder;
        }

        public void Close()
        {
            if (!IsActive)
                throw new InvalidOperationException("Auction is already closed");

            EndTime = DateTime.UtcNow;
        }

        private void ValidateBidder(string bidder)
        {
            if (string.IsNullOrWhiteSpace(bidder))
                throw new ArgumentException("Bidder cannot be empty", nameof(bidder));
        }

        private void ValidateAuctionActive()
        {
            if (!IsActive)
                throw new InvalidBidException("Cannot place bid on closed auction");
        }

        private void ValidateBidAmount(Money amount)
        {
            if (amount == null)
                throw new ArgumentNullException(nameof(amount));

            if (amount <= CurrentHighestBid)
                throw new InvalidBidException(
                    $"Bid amount ({amount}) must be greater than current highest bid ({CurrentHighestBid})");
        }

        public AuctionSummary GetSummary()
        {
            return new AuctionSummary(
                VehicleId,
                StartTime,
                EndTime,
                CurrentHighestBid,
                CurrentHighestBidder,
                _bids.Count,
                IsActive
            );
        }
    }

    public record AuctionSummary(
        VehicleId VehicleId,
        DateTime StartTime,
        DateTime? EndTime,
        Money CurrentHighestBid,
        string? CurrentHighestBidder,
        int TotalBids,
        bool IsActive
    );
}