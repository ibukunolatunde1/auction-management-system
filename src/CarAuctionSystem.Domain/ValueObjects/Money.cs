using System;

namespace CarAuctionSystem.Domain.ValueObjects
{
    public record Money
    {
        // private static readonly HashSet<string> SupportedCurrencies = new() { "USD", "EUR", "GBP"};

        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency = "USD")
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be empty", nameof(currency));

            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        public static Money operator +(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException($"Cannot add different currencies: {left.Currency} and {right.Currency}");
            return new Money(left.Amount + right.Amount, left.Currency);
        }

        public static bool operator >(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException($"Cannot compare different currencies: {left.Currency} and {right.Currency}");
            return left.Amount > right.Amount;
        }

        public static bool operator <(Money left, Money right) => !(left > right) && left != right;
        public static bool operator >=(Money left, Money right) => left > right || left == right;
        public static bool operator <=(Money left, Money right) => left < right || left == right;

        public override string ToString() => $"{Amount:0,0.00} {Currency}";
    }
}