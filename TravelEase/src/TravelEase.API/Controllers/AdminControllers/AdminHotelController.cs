using Microsoft.AspNetCore.Mvc;
using TravelEase.TravelEase.Application.DTOs.Admin;
using TravelEase.TravelEase.Application.Interfaces.Admin;

namespace TravelEase.TravelEase.API.Controllers.Admin;

[ApiController]
[Route("api/admin/hotels")]
public class AdminHotelController : ControllerBase
{
    private readonly IAdminHotelService _service;

    public AdminHotelController(IAdminHotelService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id) => Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AdminHotelDto dto) => Ok(await _service.CreateAsync(dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AdminHotelDto dto) => Ok(await _service.UpdateAsync(id, dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) => Ok(await _service.DeleteAsync(id));
    
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? name, [FromQuery] string? city, [FromQuery] int? minStars)
        => Ok(await _service.GetAllAsync(name, city, minStars));


}