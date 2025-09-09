using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Entities;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripsController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trips = await _tripService.GetAllTripsAsync();
            return Ok(trips);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var trip = await _tripService.GetTripByIdAsync(id);
            if (trip == null) return NotFound();
            return Ok(trip);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Trip trip)
        {
            await _tripService.AddTripAsync(trip);
            return CreatedAtAction(nameof(GetById), new { id = trip.TripID }, trip);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Trip trip)
        {
            if (id != trip.TripID) return BadRequest();
            var updatedTrip = await _tripService.UpdateTripAsync(trip);
            return Ok(updatedTrip);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _tripService.DeleteTripAsync(id);
            return NoContent();
        }
    }
}

