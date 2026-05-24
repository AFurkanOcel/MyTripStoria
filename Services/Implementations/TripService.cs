using Contracts.TripDtos;
using Entities;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;

        public TripService(ITripRepository tripRepository)
        {
            _tripRepository = tripRepository;
        }

        public async Task<List<TripDto>> GetAllTripsAsync()
        {
            var trips = await _tripRepository.GetAllAsync();
            return trips.Select(MapTrip).ToList();
        }

        public async Task<TripDto?> GetTripByIdAsync(int tripId)
        {
            var trip = await _tripRepository.GetByIdWithDetailsAsync(tripId);
            return trip == null ? null : MapTrip(trip);
        }

        public async Task<List<TripDto>> GetTripsByUserIdAsync(int userId)
        {
            var trips = await _tripRepository.GetAllByUserIdAsync(userId);
            return trips.Select(MapTrip).ToList();
        }

        public async Task<List<TripDto>> GetUpcomingTripsByUserIdAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;
            var trips = await _tripRepository.GetAllByUserIdAsync(userId);

            return trips
                .Where(t => t.StartDate.Date >= today && t.Status != TripStatus.Cancelled)
                .OrderBy(t => t.StartDate)
                .Select(MapTrip)
                .ToList();
        }

        public async Task<List<TripDto>> GetCompletedTripsByUserIdAsync(int userId)
        {
            var trips = await _tripRepository.GetAllByUserIdAsync(userId);

            return trips
                .Where(t => t.Status == TripStatus.Completed || t.IsCompleted)
                .OrderByDescending(t => t.EndDate)
                .Select(MapTrip)
                .ToList();
        }

        public async Task<List<TripMapMarkerDto>> GetMapMarkersByUserIdAsync(int userId)
        {
            var trips = await _tripRepository.GetAllByUserIdAsync(userId);

            return trips
                .Where(t => t.Latitude.HasValue && t.Longitude.HasValue)
                .Select(t => new TripMapMarkerDto
                {
                    TripID = t.TripID,
                    UserId = t.UserID,
                    Title = t.Title,
                    Status = t.Status.ToString(),
                    PlaceName = t.PlaceName,
                    CityName = t.City?.Name,
                    CountryName = t.Country?.Name,
                    Latitude = t.Latitude!.Value,
                    Longitude = t.Longitude!.Value,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate
                })
                .ToList();
        }

        public async Task<TripDashboardSummaryDto> GetDashboardSummaryByUserIdAsync(int userId)
        {
            var trips = await _tripRepository.GetAllByUserIdAsync(userId);
            var activeUpcomingTrips = trips
                .Where(t => t.StartDate >= DateTime.UtcNow.Date && t.Status != TripStatus.Cancelled)
                .OrderBy(t => t.StartDate)
                .ToList();

            return new TripDashboardSummaryDto
            {
                TotalTrips = trips.Count,
                PlannedTrips = trips.Count(t => t.Status == TripStatus.Planned),
                OngoingTrips = trips.Count(t => t.Status == TripStatus.Ongoing),
                CompletedTrips = trips.Count(t => t.Status == TripStatus.Completed || t.IsCompleted),
                CancelledTrips = trips.Count(t => t.Status == TripStatus.Cancelled),
                TotalPlannedBudget = trips.Sum(t => t.PlannedBudget ?? 0),
                TotalActualCost = trips.Sum(t => t.ActualCost ?? 0),
                NextTripStartDate = activeUpcomingTrips.FirstOrDefault()?.StartDate
            };
        }

        public async Task AddTripAsync(Trip trip)
        {
            trip.CreatedAt = DateTime.UtcNow;
            trip.Status = NormalizeStatus(trip.Status, trip.IsCompleted);
            trip.IsCompleted = trip.Status == TripStatus.Completed;
            await _tripRepository.AddAsync(trip);
        }

        public async Task DeleteTripAsync(int tripId)
        {
            await _tripRepository.DeleteAsync(tripId);
        }

        public async Task<Trip> UpdateTripAsync(Trip trip)
        {
            trip.UpdatedAt = DateTime.UtcNow;
            trip.Status = NormalizeStatus(trip.Status, trip.IsCompleted);
            trip.IsCompleted = trip.Status == TripStatus.Completed;
            return await _tripRepository.UpdateAsync(trip);
        }

        private static TripStatus NormalizeStatus(TripStatus status, bool isCompleted)
        {
            return isCompleted ? TripStatus.Completed : status;
        }

        private static TripDto MapTrip(Trip trip)
        {
            return new TripDto
            {
                TripID = trip.TripID,
                UserId = trip.UserID,
                IsCompleted = trip.IsCompleted,
                Title = trip.Title,
                Description = trip.Description,
                TripType = trip.TripType,
                Status = trip.Status.ToString(),
                Visibility = trip.Visibility.ToString(),
                CountryId = trip.CountryId,
                CityId = trip.CityId,
                CountryName = trip.Country?.Name,
                CityName = trip.City?.Name,
                PlaceName = trip.PlaceName,
                Address = trip.Address,
                Latitude = trip.Latitude,
                Longitude = trip.Longitude,
                CoverImageUrl = trip.CoverImageUrl,
                Rating = trip.Rating,
                PlannedBudget = trip.PlannedBudget,
                ActualCost = trip.ActualCost,
                FavoriteMoments = trip.FavoriteMoments,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Notes = trip.Notes,
                Waypoints = trip.Waypoints
                    .OrderBy(w => w.SortOrder)
                    .Select(w => new TripWaypointDto
                    {
                        Id = w.Id,
                        Name = w.Name,
                        Address = w.Address,
                        Latitude = w.Latitude,
                        Longitude = w.Longitude,
                        SortOrder = w.SortOrder,
                        PlannedAt = w.PlannedAt,
                        Notes = w.Notes
                    })
                    .ToList()
            };
        }
    }
}
