using System;
using System.Collections.Generic;

namespace CarAuctionSystem.Application.DTOs
{
    public record VehicleDto
    (
        string Id,
        string Vin,
        string Type,
        string Manufacturer,
        string Model,
        int Year,
        decimal StartingBidAmount,
        string StartingBidCurrency,
        DateTime CreatedAt,
        Dictionary<string, object> AdditionalAttributes
    );

    public record CreateVehicleRequest
    (
        string Id,
        string Vin,
        string Type,
        string Manufacturer,
        string Model,
        int Year,
        decimal StartingBidAmount,
        string StartingBidCurrency,
        Dictionary<string, object> AdditionalAttributes
    );

    public record VehicleSearchRequest(
        string? Type = null,
        string? Manufacturer = null,
        string? Model = null,
        int? Year = null,
        int? MinYear = null,
        int? MaxYear = null,
        int Skip = 0,
        int Take = 10,
        string? Vin = null
    );

    public record VehicleSearchResponse(
        IEnumerable<VehicleDto> Vehicles,
        int TotalCount,
        string SearchDescription
    );
}