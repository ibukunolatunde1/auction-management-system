using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CarAuctionSystem.Application.DTOs;
using CarAuctionSystem.Domain.Services.SearchCriteria;

namespace CarAuctionSystem.Application.Services
{
    public interface IAuctionService
    {
        // Vehicle Management
        Task<VehicleDto> AddVehicleAsync(CreateVehicleRequest request, CancellationToken cancellationToken = default);
        Task<VehicleSearchResponse> SearchVehiclesAsync(VehicleSearchRequest request, CancellationToken cancellationToken = default);
        Task<VehicleDto?> GetVehicleByIdAsync(string vehicleId, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetSupportedVehicleTypesAsync(CancellationToken cancellationToken = default);

        // Auction Management
        Task<AuctionDto> StartAuctionAsync(StartAuctionRequest request, CancellationToken cancellationToken = default);
        Task<AuctionDto> PlaceBidAsync(PlaceBidRequest request, CancellationToken cancellationToken = default);
        Task<AuctionSummaryDto> CloseAuctionAsync(CloseAuctionRequest request, CancellationToken cancellationToken = default);
        Task<AuctionDto?> GetActiveAuctionAsync(string vehicleId, CancellationToken cancellationToken = default);
        Task<IEnumerable<AuctionDto>> GetAllActiveAuctionsAsync(CancellationToken cancellationToken = default);

        // Statistics
        Task<int> GetVehicleCountAsync(CancellationToken cancellationToken = default);
        Task<int> GetActiveAuctionCountAsync(CancellationToken cancellationToken = default);
    }
}