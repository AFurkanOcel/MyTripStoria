using System.Security.Claims;
using Contracts.TripDtos;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripService;
        private readonly IUserService _userService;
        private readonly ICountryService _countryService;
        private readonly ICityService _cityService;

        public TripsController(
            ITripService tripService,
            IUserService userService,
            ICountryService countryService,
            ICityService cityService)
        {
            _tripService = tripService;
            _userService = userService;
            _countryService = countryService;
            _cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var profile = await GetCurrentProfileAsync();
            if (profile == null)
                return NotFound("A profile has not been created for the current identity user.");

            var trips = await _tripService.GetTripsByUserIdAsync(profile.Id);
            return Ok(trips);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var trip = await _tripService.GetTripByIdAsync(id);
            if (trip == null)
                return NotFound($"The trip with ID {id} was not found.");

            if (!await CanAccessUserAsync(trip.UserId))
                return Forbid();

            return Ok(trip);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            if (!await CanAccessUserAsync(userId))
                return Forbid();

            var trips = await _tripService.GetTripsByUserIdAsync(userId);
            return Ok(trips);
        }

        [HttpGet("by-user/{userId}/upcoming")]
        public async Task<IActionResult> GetUpcomingByUserId(int userId)
        {
            if (!await CanAccessUserAsync(userId))
                return Forbid();

            return Ok(await _tripService.GetUpcomingTripsByUserIdAsync(userId));
        }

        [HttpGet("by-user/{userId}/completed")]
        public async Task<IActionResult> GetCompletedByUserId(int userId)
        {
            if (!await CanAccessUserAsync(userId))
                return Forbid();

            return Ok(await _tripService.GetCompletedTripsByUserIdAsync(userId));
        }

        [HttpGet("by-user/{userId}/map-markers")]
        public async Task<IActionResult> GetMapMarkersByUserId(int userId)
        {
            if (!await CanAccessUserAsync(userId))
                return Forbid();

            return Ok(await _tripService.GetMapMarkersByUserIdAsync(userId));
        }

        [HttpGet("by-user/{userId}/summary")]
        public async Task<IActionResult> GetDashboardSummaryByUserId(int userId)
        {
            if (!await CanAccessUserAsync(userId))
                return Forbid();

            return Ok(await _tripService.GetDashboardSummaryByUserIdAsync(userId));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TripPostDto tripPostDto)
        {
            var validationResult = await ValidateTripRequestAsync(tripPostDto.UserId, tripPostDto.CountryId, tripPostDto.CityId, tripPostDto.StartDate, tripPostDto.EndDate);
            if (validationResult != null)
                return validationResult;

            var trip = MapTrip(tripPostDto);
            await _tripService.AddTripAsync(trip);
            return CreatedAtAction(nameof(GetById), new { id = trip.TripID }, await _tripService.GetTripByIdAsync(trip.TripID));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TripPutDto tripPutDto)
        {
            var existingTrip = await _tripService.GetTripByIdAsync(id);
            if (existingTrip == null)
                return NotFound($"The trip with ID {id} was not found.");

            if (!await CanAccessUserAsync(existingTrip.UserId))
                return Forbid();

            var validationResult = await ValidateTripRequestAsync(tripPutDto.UserId, tripPutDto.CountryId, tripPutDto.CityId, tripPutDto.StartDate, tripPutDto.EndDate);
            if (validationResult != null)
                return validationResult;

            var trip = MapTrip(tripPutDto);
            trip.TripID = id;

            var updatedTrip = await _tripService.UpdateTripAsync(trip);
            return Ok(await _tripService.GetTripByIdAsync(updatedTrip.TripID));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var trip = await _tripService.GetTripByIdAsync(id);
            if (trip == null)
                return NotFound($"The trip with ID {id} was not found.");

            if (!await CanAccessUserAsync(trip.UserId))
                return Forbid();

            await _tripService.DeleteTripAsync(id);
            return Ok($"The Trip '{trip.Title}' (ID:{id}) has been deleted.");
        }

        private async Task<IActionResult?> ValidateTripRequestAsync(int userId, int countryId, int cityId, DateTime startDate, DateTime endDate)
        {
            if (!await CanAccessUserAsync(userId))
                return Forbid();

            if (endDate < startDate)
                return BadRequest("EndDate must be greater than or equal to StartDate.");

            var country = await _countryService.GetCountryByIdAsync(countryId);
            if (country == null)
                return BadRequest($"The country with ID {countryId} was not found.");

            var city = await _cityService.GetCityByIdAsync(cityId);
            if (city == null)
                return BadRequest($"The city with ID {cityId} was not found.");

            if (city.CountryId != countryId)
                return BadRequest($"The city with ID {cityId} does not belong to the country with ID {countryId}.");

            return null;
        }

        private async Task<bool> CanAccessUserAsync(int userId)
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(identityUserId))
                return false;

            var profile = await _userService.GetUserByIdentityUserIdAsync(identityUserId);
            return profile?.Id == userId;
        }

        private async Task<Contracts.UserDtos.UserDto?> GetCurrentProfileAsync()
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrWhiteSpace(identityUserId)
                ? null
                : await _userService.GetUserByIdentityUserIdAsync(identityUserId);
        }

        private static Trip MapTrip(TripPostDto dto)
        {
            return new Trip
            {
                UserID = dto.UserId,
                IsCompleted = dto.IsCompleted,
                Status = ParseStatus(dto.Status, dto.IsCompleted),
                Visibility = ParseVisibility(dto.Visibility),
                Title = dto.Title,
                Description = dto.Description,
                TripType = dto.TripType,
                CountryId = dto.CountryId,
                CityId = dto.CityId,
                PlaceName = dto.PlaceName,
                Address = dto.Address,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                CoverImageUrl = dto.CoverImageUrl,
                Rating = dto.Rating,
                PlannedBudget = dto.PlannedBudget,
                ActualCost = dto.ActualCost,
                FavoriteMoments = dto.FavoriteMoments,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Notes = dto.Notes,
                Waypoints = dto.Waypoints.Select(MapWaypoint).ToList()
            };
        }

        private static Trip MapTrip(TripPutDto dto)
        {
            return new Trip
            {
                UserID = dto.UserId,
                IsCompleted = dto.IsCompleted,
                Status = ParseStatus(dto.Status, dto.IsCompleted),
                Visibility = ParseVisibility(dto.Visibility),
                Title = dto.Title,
                Description = dto.Description,
                TripType = dto.TripType,
                CountryId = dto.CountryId,
                CityId = dto.CityId,
                PlaceName = dto.PlaceName,
                Address = dto.Address,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                CoverImageUrl = dto.CoverImageUrl,
                Rating = dto.Rating,
                PlannedBudget = dto.PlannedBudget,
                ActualCost = dto.ActualCost,
                FavoriteMoments = dto.FavoriteMoments,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Notes = dto.Notes,
                Waypoints = dto.Waypoints.Select(MapWaypoint).ToList()
            };
        }

        private static TripWaypoint MapWaypoint(TripWaypointDto dto)
        {
            return new TripWaypoint
            {
                Id = dto.Id,
                Name = dto.Name,
                Address = dto.Address,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                SortOrder = dto.SortOrder,
                PlannedAt = dto.PlannedAt,
                Notes = dto.Notes
            };
        }

        private static TripStatus ParseStatus(string? status, bool isCompleted)
        {
            if (isCompleted)
                return TripStatus.Completed;

            return Enum.TryParse<TripStatus>(status, true, out var parsedStatus)
                ? parsedStatus
                : TripStatus.Planned;
        }

        private static TripVisibility ParseVisibility(string? visibility)
        {
            return Enum.TryParse<TripVisibility>(visibility, true, out var parsedVisibility)
                ? parsedVisibility
                : TripVisibility.Private;
        }
    }
}
