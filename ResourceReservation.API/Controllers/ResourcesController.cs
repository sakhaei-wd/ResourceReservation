using Microsoft.AspNetCore.Mvc;
using ResourceReservation.API.Services;

namespace ResourceReservation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResourcesController : ControllerBase
{
    private readonly ReservationService _service;

    public ResourcesController(ReservationService service)
    {
        _service = service;
    }

    /// <summary>لیست منابع به همراه نوبت‌های خالی امروز</summary>
    [HttpGet("available-slots")]
    public async Task<IActionResult> GetAvailableSlots()
    {
        var result = await _service.GetResourcesWithAvailableSlotsAsync();
        return Ok(result);
    }
}