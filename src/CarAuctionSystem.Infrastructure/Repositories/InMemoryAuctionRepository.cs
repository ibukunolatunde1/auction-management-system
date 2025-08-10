using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CarAuctionSystem.Application.Interfaces.Repositories;
using CarAuctionSystem.Domain.Entities;
using CarAuctionSystem.Domain.ValueObjects;

namespace CarAuctionSystem.Infrastructure.Repositories
{
    public class InMemoryAuctionRepository : IAuctionRepository
    {
        private readonly ConcurrentDictionary<string, List<Auction>> _auctionsByVehicle = new();
        private readonly object _lock = new();

        public Task<Auction?> GetActiveAuctionAsync(VehicleId vehicleId, CancellationToken cancellationToken = default)
        {
            if (vehicleId == null) throw new ArgumentNullException(nameof(vehicleId));
            cancellationToken.ThrowIfCancellationRequested();

            lock (_lock)
            {
                if (_auctionsByVehicle.TryGetValue(vehicleId.Value, out var auctions))
                {
                    return Task.FromResult(auctions.FirstOrDefault(a => a.IsActive));
                }
            }

            return Task.FromResult<Auction?>(null);
        }

        public Task<IEnumerable<Auction>> GetAllActiveAuctionsAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            lock (_lock)
            {
                var activeAuctions = _auctionsByVehicle.Values
                    .SelectMany(auctions => auctions)
                    .Where(a => a.IsActive)
                    .OrderByDescending(a => a.StartTime);

                // return Task.FromResult(activeAuctions);
                return Task.FromResult<IEnumerable<Auction>>(activeAuctions);
            }
        }

        public Task<Auction?> GetAuctionByIdAsync(VehicleId vehicleId, CancellationToken cancellationToken = default)
        {
            if (vehicleId == null) throw new ArgumentNullException(nameof(vehicleId));
            cancellationToken.ThrowIfCancellationRequested();

            lock (_lock)
            {
                if (_auctionsByVehicle.TryGetValue(vehicleId.Value, out var auctions))
                {
                    return Task.FromResult(auctions.LastOrDefault());
                }
            }

            return Task.FromResult<Auction?>(null);
        }

        public Task AddAsync(Auction auction, CancellationToken cancellationToken = default)
        {
            if (auction == null) throw new ArgumentNullException(nameof(auction));
            cancellationToken.ThrowIfCancellationRequested();

            lock (_lock)
            {
                var vehicleId = auction.VehicleId.Value;
                
                if (!_auctionsByVehicle.TryGetValue(vehicleId, out var auctions))
                {
                    auctions = new List<Auction>();
                    _auctionsByVehicle[vehicleId] = auctions;
                }

                // Ensure only one active auction per vehicle
                var existingActiveAuction = auctions.FirstOrDefault(a => a.IsActive);
                if (existingActiveAuction != null)
                {
                    throw new InvalidOperationException($"Vehicle '{vehicleId}' already has an active auction");
                }

                auctions.Add(auction);
            }

            return Task.CompletedTask;
        }

        public Task UpdateAsync(Auction auction, CancellationToken cancellationToken = default)
        {
            if (auction == null) throw new ArgumentNullException(nameof(auction));
            cancellationToken.ThrowIfCancellationRequested();

            // In-memory implementation doesn't need explicit updates since objects are references
            // In a real database implementation, this would persist changes
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Auction>> GetAuctionHistoryAsync(VehicleId vehicleId, CancellationToken cancellationToken = default)
        {
            if (vehicleId == null) throw new ArgumentNullException(nameof(vehicleId));
            cancellationToken.ThrowIfCancellationRequested();

            lock (_lock)
            {
                if (_auctionsByVehicle.TryGetValue(vehicleId.Value, out var auctions))
                {
                    var history = auctions.OrderByDescending(a => a.StartTime);
                    // return Task.FromResult(history);
                    return Task.FromResult<IEnumerable<Auction>>(history);

                }
            }

            return Task.FromResult(Enumerable.Empty<Auction>());
        }

        public Task<int> GetActiveAuctionCountAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            lock (_lock)
            {
                var count = _auctionsByVehicle.Values
                    .SelectMany(auctions => auctions)
                    .Count(a => a.IsActive);

                return Task.FromResult(count);
            }
        }

        // Additional methods for testing and diagnostics
        public Task ClearAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            lock (_lock)
            {
                _auctionsByVehicle.Clear();
            }
            
            return Task.CompletedTask;
        }

        public Task<int> GetTotalAuctionCountAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            lock (_lock)
            {
                var count = _auctionsByVehicle.Values
                    .SelectMany(auctions => auctions)
                    .Count();

                return Task.FromResult(count);
            }
        }

        public Task<IEnumerable<Auction>> GetCompletedAuctionsAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            lock (_lock)
            {
                var completedAuctions = _auctionsByVehicle.Values
                    .SelectMany(auctions => auctions)
                    .Where(a => !a.IsActive)
                    .OrderByDescending(a => a.EndTime);

                // return Task.FromResult(completedAuctions);
                return Task.FromResult<IEnumerable<Auction>>(completedAuctions);
            }
        }
    }
}