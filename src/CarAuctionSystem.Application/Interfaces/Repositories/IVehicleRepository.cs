using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using CarAuctionSystem.Domain.Entities;
using CarAuctionSystem.Domain.ValueObjects;
using CarAuctionSystem.Domain.Services.SearchCriteria;

namespace CarAuctionSystem.Application.Interfaces.Repositories
{
    public interface IVehicleRepository
    {
        Task<bool> ExistsAsync(VehicleId vehicleId, CancellationToken cancellationToken = default);
        Task<Vehicle?> GetByIdAsync(VehicleId vehicleId, CancellationToken cancellationToken = default);
        // Task<Vehicle?> GetByVinAsync(Vin vin, CancellationToken cancellationToken = default);
        Task<IEnumerable<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Vehicle>> SearchAsync(SearchCriteria criteria, CancellationToken cancellationToken = default);
        Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
        Task<int> GetCountAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Vehicle>> GetPagedAsync(int skip, int take, CancellationToken cancellationToken = default);
    }
}