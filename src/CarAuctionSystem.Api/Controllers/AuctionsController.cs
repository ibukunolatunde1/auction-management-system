using CarAuctionSystem.Application.DTOs;
using CarAuctionSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarAuctionSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController : ControllerBase
{
    private readonly IAuctionService _auctionService;

    public AuctionsController(IAuctionService auctionService)
    {
        _auctionService = auctionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuctionDto>>> GetAllAuctions(CancellationToken cancellationToken)
    {
        var auctions = await _auctionService.GetAllActiveAuctionsAsync(cancellationToken);
        return Ok(auctions);
    }

    [HttpGet("{vehicleId}")]
    public async Task<ActionResult<AuctionDto>> GetAuction(string vehicleId, CancellationToken cancellationToken)
    {
        var auction = await _auctionService.GetActiveAuctionAsync(vehicleId, cancellationToken);
        if (auction == null)
            return NotFound();
        
        return Ok(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> StartAuction([FromBody] StartAuctionRequest request, CancellationToken cancellationToken)
    {

        var createdAuction = await _auctionService.StartAuctionAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAuction), new { vehicleId = createdAuction.VehicleId }, createdAuction);
    }

    [HttpPost("bid")]
    public async Task<ActionResult<AuctionDto>> PlaceBid([FromBody] PlaceBidRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var auction = await _auctionService.PlaceBidAsync(request, cancellationToken);
            return Ok(auction);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{vehicleId}/close")]
    public async Task<ActionResult<AuctionSummaryDto>> CloseAuction(string vehicleId, CancellationToken cancellationToken)
    {
        try
        {
            var request = new CloseAuctionRequest(vehicleId);
            var summary = await _auctionService.CloseAuctionAsync(request, cancellationToken);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }}
