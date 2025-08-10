using CarAuctionSystem.Application.DTOs;
using CarAuctionSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarAuctionSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IAuctionService _auctionService;

    public VehiclesController(IAuctionService auctionService)
    {
        _auctionService = auctionService;
    }

    [HttpGet]
    public async Task<ActionResult<VehicleSearchResponse>> SearchVehicles([FromQuery] string? type = null, [FromQuery] string? manufacturer = null,
        [FromQuery] string? model = null, [FromQuery] int? year = null, [FromQuery] int? minYear = null, [FromQuery] int? maxYear = null,
        [FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string? vin = null, CancellationToken cancellationToken = default)
    {
        var request = new VehicleSearchRequest(type, manufacturer, model, year, minYear, maxYear, skip, take, vin);
        var response = await _auctionService.SearchVehiclesAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleDto>> GetVehicle(string id, CancellationToken cancellationToken)
    {
        var vehicle = await _auctionService.GetVehicleByIdAsync(id, cancellationToken);
        if (vehicle == null)
            return NotFound();
            
        return Ok(vehicle);
    }

    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<string>>> GetVehicleTypes(CancellationToken cancellationToken)
    {
        var types = await _auctionService.GetSupportedVehicleTypesAsync(cancellationToken);
        return Ok(types);
    }

    [HttpPost]
    public async Task<ActionResult<VehicleDto>> CreateVehicle([FromBody] CreateVehicleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var vehicle = await _auctionService.AddVehicleAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id }, vehicle);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
