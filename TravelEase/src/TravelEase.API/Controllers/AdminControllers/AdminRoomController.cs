using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelEase.Application.DTOs.Admin;
using TravelEase.Application.Interfaces.Admin;

namespace TravelEase.API.Controllers.AdminControllers;

[ApiController]
[Route("api/admin/rooms")]
[Authorize(Roles = "Admin")] // Require JWT with Admin role
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
    {
        var rooms = await _service.GetAllAsync(hotelName, isAvailable, minAdults, minChildren);
        return Ok(rooms);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var room = await _service.GetByIdAsync(id);
        return Ok(room);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AdminRoomDto dto)
    {
        var createdRoom = await _service.CreateAsync(dto);
        return Ok(createdRoom);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AdminRoomDto dto)
    {
        var updatedRoom = await _service.UpdateAsync(id, dto);
        return Ok(updatedRoom);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return Ok(result);
    }
}