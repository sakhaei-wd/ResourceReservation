using Microsoft.AspNetCore.Mvc;
using ResourceReservation.API.Services;
using ResourceReservation.Core.DTOs;

namespace ResourceReservation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly ReservationService _service;

    public ReservationsController(ReservationService service)
    {
        _service = service;
    }

    /// <summary>ایجاد رزرو جدید</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetReport), new { }, result);
    }

    /// <summary>لغو رزرو</summary>
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        await _service.CancelAsync(id);
        return NoContent();
    }

    /// <summary>گزارش رزروهای یک منبع در بازه زمانی</summary>
    [HttpGet("report")]
    public async Task<IActionResult> GetReport(
        [FromQuery] int resourceId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var result = await _service.GetReportAsync(resourceId, from, to);
        return Ok(result);
    }
}