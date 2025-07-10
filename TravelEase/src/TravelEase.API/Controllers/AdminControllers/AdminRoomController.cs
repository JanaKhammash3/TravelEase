using Microsoft.AspNetCore.Mvc;
using TravelEase.TravelEase.Application.DTOs.Admin;
using TravelEase.TravelEase.Application.Interfaces.Admin;

namespace TravelEase.TravelEase.API.Controllers.Admin;

[ApiController]
[Route("api/admin/rooms")]
public class AdminRoomController : ControllerBase
{
    private readonly IAdminRoomService _service;

    public AdminRoomController(IAdminRoomService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? hotelName,
        [FromQuery] bool? isAvailable,
        [FromQuery] int? minAdults,
        [FromQuery] int? minChildren)
        => Ok(await _service.GetAllAsync(hotelName, isAvailable, minAdults, minChildren));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id) => Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AdminRoomDto dto) => Ok(await _service.CreateAsync(dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AdminRoomDto dto) => Ok(await _service.UpdateAsync(id, dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) => Ok(await _service.DeleteAsync(id));
}