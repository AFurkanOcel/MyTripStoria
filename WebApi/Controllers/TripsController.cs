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
        private const long MaxPhotoSizeInBytes = 5 * 1024 * 1024;
        private static readonly HashSet<string> AllowedPhotoExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        private readonly ITripService _tripService;
        private readonly IUserService _userService;
        private readonly ICountryService _countryService;
        private readonly ICityService _cityService;
        private readonly IWebHostEnvironment _environment;

        public TripsController(
            ITripService tripService,
            IUserService userService,
            ICountryService countryService,
            ICityService cityService,
            IWebHostEnvironment environment)
        {
            _tripService = tripService;
            _userService = userService;
            _countryService = countryService;
            _cityService = cityService;
            _environment = environment;
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

        [HttpGet("me")]
        public async Task<IActionResult> GetMine()
        {
            var profile = await GetCurrentProfileAsync();
            if (profile == null)
                return NotFound("A profile has not been created for the current identity user.");

            return Ok(await _tripService.GetTripsByUserIdAsync(profile.Id));
        }

        [HttpGet("me/planned")]
        public async Task<IActionResult> GetMyPlanned()
        {
            var profile = await GetCurrentProfileAsync();
            if (profile == null)
                return NotFound("A profile has not been created for the current identity user.");

            return Ok(await _tripService.GetPlannedTripsByUserIdAsync(profile.Id));
        }

        [HttpGet("me/upcoming")]
        public async Task<IActionResult> GetMyUpcoming()
        {
            var profile = await GetCurrentProfileAsync();
            if (profile == null)
                return NotFound("A profile has not been created for the current identity user.");

            return Ok(await _tripService.GetUpcomingTripsByUserIdAsync(profile.Id));
        }

        [HttpGet("me/completed")]
        public async Task<IActionResult> GetMyCompleted()
        {
            var profile = await GetCurrentProfileAsync();
            if (profile == null)
                return NotFound("A profile has not been created for the current identity user.");

            return Ok(await _tripService.GetCompletedTripsByUserIdAsync(profile.Id));
        }

        [HttpGet("me/map-markers")]
        public async Task<IActionResult> GetMyMapMarkers()
        {
            var profile = await GetCurrentProfileAsync();
            if (profile == null)
                return NotFound("A profile has not been created for the current identity user.");

            return Ok(await _tripService.GetMapMarkersByUserIdAsync(profile.Id));
        }

        [HttpGet("me/summary")]
        public async Task<IActionResult> GetMyDashboardSummary()
        {
            var profile = await GetCurrentProfileAsync();
            if (profile == null)
                return NotFound("A profile has not been created for the current identity user.");

            return Ok(await _tripService.GetDashboardSummaryByUserIdAsync(profile.Id));
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
            var profile = await GetCurrentProfileAsync();
            if (profile == null)
                return NotFound("A profile has not been created for the current identity user.");

            var validationResult = await ValidateTripRequestAsync(
                profile,
                tripPostDto.UserId,
                tripPostDto.CountryId,
                tripPostDto.CityId,
                tripPostDto.StartDate,
                tripPostDto.EndDate,
                tripPostDto.Latitude,
                tripPostDto.Longitude,
                tripPostDto.Photos);
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

            var profile = await GetCurrentProfileAsync();
            if (profile == null)
                return NotFound("A profile has not been created for the current identity user.");

            var validationResult = await ValidateTripRequestAsync(
                profile,
                tripPutDto.UserId,
                tripPutDto.CountryId,
                tripPutDto.CityId,
                tripPutDto.StartDate,
                tripPutDto.EndDate,
                tripPutDto.Latitude,
                tripPutDto.Longitude,
                tripPutDto.Photos);
            if (validationResult != null)
                return validationResult;

            var trip = MapTrip(tripPutDto);
            trip.TripID = id;

            var updatedTrip = await _tripService.UpdateTripAsync(trip);
            return Ok(await _tripService.GetTripByIdAsync(updatedTrip.TripID));
        }

        [HttpPost("{id}/photos")]
        [RequestSizeLimit(MaxPhotoSizeInBytes)]
        public async Task<IActionResult> UploadPhoto(
            int id,
            [FromForm] IFormFile file,
            [FromForm] string? caption,
            [FromForm] bool isCover = false,
            [FromForm] DateTime? takenAt = null)
        {
            var trip = await _tripService.GetTripByIdAsync(id);
            if (trip == null)
                return NotFound($"The trip with ID {id} was not found.");

            var profile = await GetCurrentProfileAsync();
            if (profile == null)
                return NotFound("A profile has not been created for the current identity user.");

            if (profile.Id != trip.UserId)
                return Forbid();

            if (!profile.IsPremium)
                return StatusCode(StatusCodes.Status403Forbidden, "Photo upload is available for premium users.");

            if (file == null || file.Length == 0)
                return BadRequest("A photo file is required.");

            if (file.Length > MaxPhotoSizeInBytes)
                return BadRequest("Photo size must be 5 MB or less.");

            var extension = Path.GetExtension(file.FileName);
            if (!AllowedPhotoExtensions.Contains(extension))
                return BadRequest("Only JPG, PNG, and WEBP photos are supported.");

            var uploadRoot = GetUploadRootPath();
            var relativeFolder = Path.Combine("uploads", "trips", id.ToString());
            var absoluteFolder = Path.Combine(uploadRoot, relativeFolder);
            Directory.CreateDirectory(absoluteFolder);

            var storedFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
            var absolutePath = Path.Combine(absoluteFolder, storedFileName);

            await using (var stream = System.IO.File.Create(absolutePath))
            {
                await file.CopyToAsync(stream);
            }

            var photo = new TripPhoto
            {
                Url = "/" + Path.Combine(relativeFolder, storedFileName).Replace("\\", "/"),
                OriginalFileName = Path.GetFileName(file.FileName),
                ContentType = file.ContentType,
                SizeInBytes = file.Length,
                Caption = caption,
                IsCover = isCover,
                SortOrder = trip.Photos.Count,
                TakenAt = takenAt
            };

            var savedPhoto = await _tripService.AddPhotoAsync(id, photo);
            return CreatedAtAction(nameof(GetById), new { id }, savedPhoto);
        }

        [HttpDelete("{tripId}/photos/{photoId}")]
        public async Task<IActionResult> DeletePhoto(int tripId, int photoId)
        {
            var trip = await _tripService.GetTripByIdAsync(tripId);
            if (trip == null)
                return NotFound($"The trip with ID {tripId} was not found.");

            if (!await CanAccessUserAsync(trip.UserId))
                return Forbid();

            var deletedPhoto = await _tripService.DeletePhotoAsync(tripId, photoId);
            if (deletedPhoto == null)
                return NotFound($"The photo with ID {photoId} was not found.");

            DeleteUploadedFileIfExists(deletedPhoto.Url);
            return Ok(deletedPhoto);
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

        private async Task<IActionResult?> ValidateTripRequestAsync(
            Contracts.UserDtos.UserDto profile,
            int userId,
            int countryId,
            int cityId,
            DateTime startDate,
            DateTime endDate,
            decimal? latitude,
            decimal? longitude,
            IReadOnlyCollection<TripPhotoDto>? photos)
        {
            if (profile.Id != userId)
                return Forbid();

            if (endDate < startDate)
                return BadRequest("EndDate must be greater than or equal to StartDate.");

            if (!profile.IsPremium && photos?.Any() == true)
                return StatusCode(StatusCodes.Status403Forbidden, "Trip photo metadata is available for premium users.");

            if (latitude.HasValue != longitude.HasValue)
                return BadRequest("Latitude and Longitude must be provided together.");

            if (photos?.Count(p => p.IsCover) > 1)
                return BadRequest("Only one photo can be marked as cover.");

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
                Waypoints = (dto.Waypoints ?? new()).Select(MapWaypoint).ToList(),
                Photos = (dto.Photos ?? new()).Select(MapPhoto).ToList()
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
                Waypoints = (dto.Waypoints ?? new()).Select(MapWaypoint).ToList(),
                Photos = (dto.Photos ?? new()).Select(MapPhoto).ToList()
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

        private static TripPhoto MapPhoto(TripPhotoDto dto)
        {
            return new TripPhoto
            {
                Id = dto.Id,
                Url = dto.Url,
                OriginalFileName = dto.OriginalFileName,
                ContentType = dto.ContentType,
                SizeInBytes = dto.SizeInBytes,
                Caption = dto.Caption,
                IsCover = dto.IsCover,
                SortOrder = dto.SortOrder,
                TakenAt = dto.TakenAt
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

        private string GetUploadRootPath()
        {
            if (!string.IsNullOrWhiteSpace(_environment.WebRootPath))
                return _environment.WebRootPath;

            return Path.Combine(_environment.ContentRootPath, "wwwroot");
        }

        private void DeleteUploadedFileIfExists(string url)
        {
            if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
                return;

            var relativePath = url.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            var absolutePath = Path.Combine(GetUploadRootPath(), relativePath);

            if (System.IO.File.Exists(absolutePath))
                System.IO.File.Delete(absolutePath);
        }
    }
}
