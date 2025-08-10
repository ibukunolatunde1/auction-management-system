using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CarAuctionSystem.Domain.Entities;
using CarAuctionSystem.Domain.ValueObjects;

namespace CarAuctionSystem.Application.Interfaces.Repositories
{
    public interface IAuctionRepository
    {
        Task<Auction?> GetActiveAuctionAsync(VehicleId vehicleId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Auction>> GetAllActiveAuctionsAsync(CancellationToken cancellationToken = default);
        Task<Auction?> GetAuctionByIdAsync(VehicleId vehicleId, CancellationToken cancellationToken = default); // Should it be vehicleId or auctionid.
        Task AddAsync(Auction auction, CancellationToken cancellationToken = default);
        Task UpdateAsync(Auction auction, CancellationToken cancellationToken = default);
        Task<IEnumerable<Auction>> GetAuctionHistoryAsync(VehicleId vehicleId, CancellationToken cancellationToken = default);
        Task<int> GetActiveAuctionCountAsync(CancellationToken cancellationToken = default);
    }
}