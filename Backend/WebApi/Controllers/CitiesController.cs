using System.Diagnostics.Metrics;
using Contracts.CityDtos;
using Contracts.CountryDtos;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Implementations;
using Services.Interfaces;

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
            if (city == null)
                return NotFound();
            return Ok(city);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CityPostDto cityPostDto)
        {
            if (cityPostDto.CountryId == 0)
                return BadRequest("City must have a valid CountryId.");

            // DTO -> Entity
            var city = new City
            {
                Name = cityPostDto.Name,
                CountryId = cityPostDto.CountryId
            };

            await _cityService.AddCityAsync(city);
            return CreatedAtAction(nameof(GetById), new { id = city.Id }, cityPostDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CityPutDto cityPutDto)
        {
            // DTO -> Entity
            var city = new City
            {
                Id = id,
                Name = cityPutDto.Name,
                CountryId = cityPutDto.CountryId,
            };

            var updatedCity = await _cityService.UpdateCityAsync(city);
            return Ok(cityPutDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var city = await _cityService.GetCityByIdAsync(id);
            if (city == null)
                return NotFound(); //204

            await _cityService.DeleteCityAsync(id);
            return Ok($"The city {city.Name} (ID:{id}) has been deleted.");
        }
    }
}
