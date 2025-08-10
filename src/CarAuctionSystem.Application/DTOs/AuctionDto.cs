using System;
using System.Collections.Generic;

namespace CarAuctionSystem.Application.DTOs
{
    public record AuctionDto(
        string VehicleId,
        DateTime StartTime,
        DateTime? EndTime,
        decimal CurrentHighestBidAmount,
        string CurrentHighestBidCurrency,
        string? CurrentHighestBidder,
        int TotalBids,
        bool IsActive,
        IEnumerable<BidDto> RecentBids
    );

    public record BidDto(
        string Bidder,
        decimal Amount,
        string Currency,
        DateTime PlacedAt
    );

    public record StartAuctionRequest(string VehicleId);

    public record PlaceBidRequest(
        string VehicleId,
        string Bidder,
        decimal Amount,
        string Currency = "USD"
    );

    public record CloseAuctionRequest(string VehicleId);
    
    public record AuctionSummaryDto(
        string VehicleId,
        DateTime StartTime,
        DateTime? EndTime,
        decimal CurrentHighestBidAmount,
        string CurrentHighestBidCurrency,
        string? CurrentHighestBidder,
        int TotalBids,
        bool IsActive
    );
}