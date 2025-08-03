using Microsoft.AspNetCore.Mvc;
using TravelEase.Application.Features.City;
using TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Application.Interfaces;

namespace TravelEase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cities = await _cityService.GetAllCitiesAsync();
            return Ok(cities);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCityCommand command)
        {
            await _cityService.CreateCityAsync(command);
            return Ok(new { message = "City created successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _cityService.DeleteCityAsync(id);
            return Ok(new { message = "City deleted successfully" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCityCommand command)
        {
            command.Id = id;
            await _cityService.UpdateCityAsync(command);
            return Ok(new { message = "City updated successfully" });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _cityService.SearchCitiesAsync(keyword, page, pageSize);
            return Ok(result);
        }
    }
}