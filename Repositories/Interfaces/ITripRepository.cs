using Entities;

namespace Repositories.Interfaces
{
    public interface ITripRepository : IRepository<Trip>
    {
        Task<List<Trip>> GetAllByUserIdAsync(int userId);
        Task<Trip?> GetByIdWithDetailsAsync(int tripId);
        Task<TripPhoto> AddPhotoAsync(int tripId, TripPhoto photo);
        Task<TripPhoto?> DeletePhotoAsync(int tripId, int photoId);
    }
}
