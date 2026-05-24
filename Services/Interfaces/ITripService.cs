using Contracts.TripDtos;
using Entities;

namespace Services.Interfaces
{
    public interface ITripService
    {
        Task<List<TripDto>> GetAllTripsAsync();
        Task<TripDto?> GetTripByIdAsync(int tripId);
        Task<List<TripDto>> GetTripsByUserIdAsync(int userId);
        Task<List<TripDto>> GetUpcomingTripsByUserIdAsync(int userId);
        Task<List<TripDto>> GetCompletedTripsByUserIdAsync(int userId);
        Task<List<TripDto>> GetPlannedTripsByUserIdAsync(int userId);
        Task<List<TripMapMarkerDto>> GetMapMarkersByUserIdAsync(int userId);
        Task<TripDashboardSummaryDto> GetDashboardSummaryByUserIdAsync(int userId);
        Task AddTripAsync(Trip trip);
        Task<Trip> UpdateTripAsync(Trip trip);
        Task DeleteTripAsync(int tripId);
        Task<TripPhotoDto> AddPhotoAsync(int tripId, TripPhoto photo);
        Task<TripPhotoDto?> DeletePhotoAsync(int tripId, int photoId);
    }
}
