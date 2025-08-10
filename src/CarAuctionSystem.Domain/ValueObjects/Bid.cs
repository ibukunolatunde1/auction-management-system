using System;

namespace CarAuctionSystem.Domain.ValueObjects
{
    public record Bid(string Bidder, Money Amount, DateTime PlacedAt)
    {
        // public string Bidder { get;  }
        // public Money Amount { get; }
        // public DateTime PlacedAt { get; }
        public Bid(string bidder, Money amount) : this(bidder, amount, DateTime.UtcNow)
        {
            if (string.IsNullOrWhiteSpace(bidder))
                throw new ArgumentException("Bidder cannot be empty", nameof(bidder));
            if (amount == null || amount.Amount <= 0)
                throw new ArgumentNullException(nameof(amount));

            Bidder = bidder.Trim();
            Amount = amount;
            PlacedAt = PlacedAt;
        }
    }
}