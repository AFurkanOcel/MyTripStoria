using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Entities;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CitiesController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cities = await _cityService.GetAllCitiesAsync();
            return Ok(cities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var city = await _cityService.GetCityByIdAsync(id);
            if (city == null) return NotFound();
            return Ok(city);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] City city)
        {
            if (city.CountryId == 0)
                return BadRequest("City must have a valid CountryId.");

            await _cityService.AddCityAsync(city);
            return CreatedAtAction(nameof(GetById), new { id = city.Id }, city);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] City city)
        {
            if (id != city.Id) return BadRequest();
            var updatedCity = await _cityService.UpdateCityAsync(city);
            return Ok(updatedCity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _cityService.DeleteCityAsync(id);
            return NoContent();
        }
    }
}
