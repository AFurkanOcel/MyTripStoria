using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountriesController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var countries = await _countryService.GetAllCountriesAsync();
            return Ok(countries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var country = await _countryService.GetCountryByIdAsync(id);
            if (country == null)
                return NotFound();

            return Ok(country);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Country country)
        {
            await _countryService.AddCountryAsync(country);
            return CreatedAtAction(nameof(GetById), new { id = country.Id }, country);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Country country)
        {
            if (id != country.Id) return BadRequest();
            var updatedCountry = await _countryService.UpdateCountryAsync(country);
            return Ok(updatedCountry);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _countryService.DeleteCountryAsync(id);
            return NoContent();
        }
    }
}
