using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelEase.TravelEase.Application.DTOs.Admin;
using TravelEase.TravelEase.Application.Interfaces.Admin;
// 👈 Add this

namespace TravelEase.TravelEase.API.Controllers.AdminControllers;

[ApiController]
[Route("api/admin/cities")]
[Authorize(Roles = "Admin")] 
public class AdminCityController : ControllerBase {
    private readonly IAdminCityService _service;

    public AdminCityController(IAdminCityService service) {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? name,
        [FromQuery] string? country,
        [FromQuery] string? postOffice,
        [FromQuery] int? minHotels)
    {
        return Ok(await _service.GetAllAsync(name, country, postOffice, minHotels));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id) => Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create(AdminCityDto dto) => Ok(await _service.CreateAsync(dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AdminCityDto dto) => Ok(await _service.UpdateAsync(id, dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) => Ok(await _service.DeleteAsync(id));
}