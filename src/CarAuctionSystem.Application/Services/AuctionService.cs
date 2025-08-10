using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CarAuctionSystem.Application.DTOs;
using CarAuctionSystem.Application.Factories;
using CarAuctionSystem.Application.Interfaces.Repositories;
using CarAuctionSystem.Domain.Entities;
using CarAuctionSystem.Domain.Exceptions;
using CarAuctionSystem.Domain.Services.SearchCriteria;
using CarAuctionSystem.Domain.ValueObjects;

namespace CarAuctionSystem.Application.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IAuctionRepository _auctionRepository;
        private readonly IVehicleFactory _vehicleFactory;

        public AuctionService(
            IVehicleRepository vehicleRepository,
            IAuctionRepository auctionRepository,
            IVehicleFactory vehicleFactory)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _auctionRepository = auctionRepository ?? throw new ArgumentNullException(nameof(auctionRepository));
            _vehicleFactory = vehicleFactory ?? throw new ArgumentNullException(nameof(vehicleFactory));
        }

        public async Task<VehicleDto> AddVehicleAsync(CreateVehicleRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var vehicleId = new VehicleId(request.Id);
            
            if (await _vehicleRepository.ExistsAsync(vehicleId, cancellationToken))
                throw new VehicleAlreadyExistsException(request.Id);

            var money = new Money(request.StartingBidAmount, request.StartingBidCurrency);
            var vehicle = _vehicleFactory.CreateVehicle(
                request.Type, 
                request.Vin,
                vehicleId, 
                request.Manufacturer, 
                request.Model, 
                request.Year, 
                money, 
                request.AdditionalAttributes);

            await _vehicleRepository.AddAsync(vehicle, cancellationToken);

            return MapToVehicleDto(vehicle);
        }

        public async Task<VehicleSearchResponse> SearchVehiclesAsync(VehicleSearchRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var criteria = BuildSearchCriteria(request);
            var vehicles = await _vehicleRepository.SearchAsync(criteria, cancellationToken);
            var vehicleList = vehicles.ToList();
            
            var pagedVehicles = vehicleList.Skip(request.Skip).Take(request.Take);
            var vehicleDtos = pagedVehicles.Select(MapToVehicleDto);

            return new VehicleSearchResponse(
                vehicleDtos,
                vehicleList.Count,
                criteria.GetDescription()
            );
        }

        public async Task<VehicleDto?> GetVehicleByIdAsync(string vehicleId, CancellationToken cancellationToken = default)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(new VehicleId(vehicleId), cancellationToken);
            return vehicle != null ? MapToVehicleDto(vehicle) : null;
        }

        public Task<IEnumerable<string>> GetSupportedVehicleTypesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_vehicleFactory.GetSupportedVehicleTypes());
        }

        public async Task<AuctionDto> StartAuctionAsync(StartAuctionRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var vehicleId = new VehicleId(request.VehicleId);
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId, cancellationToken);
            
            if (vehicle == null)
                throw new VehicleNotFoundException(request.VehicleId);

            var existingAuction = await _auctionRepository.GetActiveAuctionAsync(vehicleId, cancellationToken);
            if (existingAuction != null)
                throw new AuctionAlreadyActiveException(request.VehicleId);

            var auction = new Auction(vehicleId, vehicle.StartingBid);
            await _auctionRepository.AddAsync(auction, cancellationToken);

            return MapToAuctionDto(auction);
        }

        public async Task<AuctionDto> PlaceBidAsync(PlaceBidRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var vehicleId = new VehicleId(request.VehicleId);
            var auction = await _auctionRepository.GetActiveAuctionAsync(vehicleId, cancellationToken);
            
            if (auction == null)
                throw new AuctionNotFoundException(request.VehicleId);

            var bidAmount = new Money(request.Amount, request.Currency);
            auction.PlaceBid(request.Bidder, bidAmount);
            
            await _auctionRepository.UpdateAsync(auction, cancellationToken);

            return MapToAuctionDto(auction);
        }

        public async Task<AuctionSummaryDto> CloseAuctionAsync(CloseAuctionRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var vehicleId = new VehicleId(request.VehicleId);
            var auction = await _auctionRepository.GetActiveAuctionAsync(vehicleId, cancellationToken);
            
            if (auction == null)
                throw new AuctionNotFoundException(request.VehicleId);

            auction.Close();
            await _auctionRepository.UpdateAsync(auction, cancellationToken);

            var summary = auction.GetSummary();
            return MapToAuctionSummaryDto(summary);
        }

        public async Task<AuctionDto?> GetActiveAuctionAsync(string vehicleId, CancellationToken cancellationToken = default)
        {
            var auction = await _auctionRepository.GetActiveAuctionAsync(new VehicleId(vehicleId), cancellationToken);
            return auction != null ? MapToAuctionDto(auction) : null;
        }

        public async Task<IEnumerable<AuctionDto>> GetAllActiveAuctionsAsync(CancellationToken cancellationToken = default)
        {
            var auctions = await _auctionRepository.GetAllActiveAuctionsAsync(cancellationToken);
            return auctions.Select(MapToAuctionDto);
        }

        public Task<int> GetVehicleCountAsync(CancellationToken cancellationToken = default)
        {
            return _vehicleRepository.GetCountAsync(cancellationToken);
        }

        public Task<int> GetActiveAuctionCountAsync(CancellationToken cancellationToken = default)
        {
            return _auctionRepository.GetActiveAuctionCountAsync(cancellationToken);
        }

        private static SearchCriteria BuildSearchCriteria(VehicleSearchRequest request)
        {
            var criteria = new List<SearchCriteria>();

            if (!string.IsNullOrWhiteSpace(request.Type))
                criteria.Add(new TypeSearchCriteria(request.Type));

            if (!string.IsNullOrWhiteSpace(request.Manufacturer))
                criteria.Add(new ManufacturerSearchCriteria(request.Manufacturer));

            if (!string.IsNullOrWhiteSpace(request.Model))
                criteria.Add(new ModelSearchCriteria(request.Model));

            if (request.Year.HasValue)
                criteria.Add(new YearSearchCriteria(request.Year.Value));
            else if (request.MinYear.HasValue || request.MaxYear.HasValue)
                criteria.Add(new YearRangeSearchCriteria(
                    request.MinYear ?? 1900, 
                    request.MaxYear ?? DateTime.Now.Year + 1));

            return criteria.Count switch
            {
                0 => new AllVehiclesSearchCriteria(),
                1 => criteria[0],
                _ => new CompositeSearchCriteria(criteria.ToArray())
            };
        }

        private static VehicleDto MapToVehicleDto(Vehicle vehicle)
        {
            return new VehicleDto(
                vehicle.Id.Value,
                vehicle.Vin,
                vehicle.GetVehicleType(),
                vehicle.Manufacturer,
                vehicle.Model,
                vehicle.Year,
                vehicle.StartingBid.Amount,
                vehicle.StartingBid.Currency,
                vehicle.CreatedAt,
                vehicle.GetSearchableAttributes()
            );
        }

        private static AuctionDto MapToAuctionDto(Auction auction)
        {
            var recentBids = auction.Bids
                .OrderByDescending(b => b.PlacedAt)
                .Take(10)
                .Select(b => new BidDto(b.Bidder, b.Amount.Amount, b.Amount.Currency, b.PlacedAt));

            return new AuctionDto(
                auction.VehicleId.Value,
                auction.StartTime,
                auction.EndTime,
                auction.CurrentHighestBid.Amount,
                auction.CurrentHighestBid.Currency,
                auction.CurrentHighestBidder,
                auction.Bids.Count,
                auction.IsActive,
                recentBids
            );
        }

        private static AuctionSummaryDto MapToAuctionSummaryDto(AuctionSummary summary)
        {
            return new AuctionSummaryDto(
                summary.VehicleId.Value,
                summary.StartTime,
                summary.EndTime,
                summary.CurrentHighestBid.Amount,
                summary.CurrentHighestBid.Currency,
                summary.CurrentHighestBidder,
                summary.TotalBids,
                summary.IsActive
            );
        }
    }
}