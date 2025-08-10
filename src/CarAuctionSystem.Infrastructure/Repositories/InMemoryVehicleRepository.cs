using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CarAuctionSystem.Application.Interfaces.Repositories;
using CarAuctionSystem.Domain.Entities;
using CarAuctionSystem.Domain.Services.SearchCriteria;
using CarAuctionSystem.Domain.ValueObjects;

namespace CarAuctionSystem.Infrastructure.Repositories
{
    public class InMemoryVehicleRepository : IVehicleRepository
    {
        private readonly ConcurrentDictionary<string, Vehicle> _vehicles = new();

        public Task<bool> ExistsAsync(VehicleId id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_vehicles.ContainsKey(id.Value));
        }

        public Task<Vehicle?> GetByIdAsync(VehicleId id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _vehicles.TryGetValue(id.Value, out var vehicle);
            return Task.FromResult(vehicle);
        }

        public Task<IEnumerable<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_vehicles.Values.AsEnumerable());
        }

        public Task<IEnumerable<Vehicle>> SearchAsync(SearchCriteria criteria, CancellationToken cancellationToken = default)
        {
            if (criteria == null) throw new ArgumentNullException(nameof(criteria));
            cancellationToken.ThrowIfCancellationRequested();

            var matchingVehicles = _vehicles.Values.Where(criteria.Matches);
            return Task.FromResult(matchingVehicles);
        }

        public Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
        {
            if (vehicle == null) throw new ArgumentNullException(nameof(vehicle));
            cancellationToken.ThrowIfCancellationRequested();

            if (!_vehicles.TryAdd(vehicle.Id.Value, vehicle))
                throw new InvalidOperationException($"Vehicle with ID '{vehicle.Id}' already exists");

            return Task.CompletedTask;
        }

        public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_vehicles.Count);
        }

        public Task<IEnumerable<Vehicle>> GetPagedAsync(int skip, int take, CancellationToken cancellationToken = default)
        {
            if (skip < 0) throw new ArgumentException("Skip cannot be negative", nameof(skip));
            if (take <= 0) throw new ArgumentException("Take must be positive", nameof(take));
            
            cancellationToken.ThrowIfCancellationRequested();

            var pagedVehicles = _vehicles.Values
                .OrderBy(v => v.CreatedAt)
                .Skip(skip)
                .Take(take);

            return Task.FromResult(pagedVehicles);
        }

        // Additional methods for testing and diagnostics
        public Task ClearAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _vehicles.Clear();
            return Task.CompletedTask;
        }

        public Task<IDictionary<string, int>> GetVehicleCountByTypeAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var counts = _vehicles.Values
                .GroupBy(v => v.GetVehicleType())
                .ToDictionary(g => g.Key, g => g.Count());

            return Task.FromResult<IDictionary<string, int>>(counts);
        }
    }
}