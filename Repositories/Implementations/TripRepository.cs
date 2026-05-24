using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class TripRepository : Repository<Trip>, ITripRepository
    {
        public TripRepository(AppDbContext context) : base(context) { }

        public override async Task<List<Trip>> GetAllAsync()
        {
            return await _context.Trips
                                 .Include(t => t.City)
                                 .Include(t => t.Country)
                                 .Include(t => t.Waypoints)
                                 .Include(t => t.Photos)
                                 .ToListAsync();
        }

        public async Task<List<Trip>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Trips
                                 .Include(t => t.City)
                                 .Include(t => t.Country)
                                 .Include(t => t.Waypoints)
                                 .Include(t => t.Photos)
                                 .Where(t => t.UserID == userId)
                                 .ToListAsync();
        }

        public async Task<Trip?> GetByIdWithDetailsAsync(int tripId)
        {
            return await _context.Trips
                                 .Include(t => t.City)
                                 .Include(t => t.Country)
                                 .Include(t => t.Waypoints)
                                 .Include(t => t.Photos)
                                 .FirstOrDefaultAsync(t => t.TripID == tripId);
        }

        public async Task<TripPhoto> AddPhotoAsync(int tripId, TripPhoto photo)
        {
            var trip = await _context.Trips
                                     .Include(t => t.Photos)
                                     .FirstOrDefaultAsync(t => t.TripID == tripId);

            if (trip == null)
                throw new KeyNotFoundException($"Trip with id {tripId} not found.");

            photo.TripId = tripId;
            if (!trip.Photos.Any())
                photo.IsCover = true;

            if (photo.IsCover)
            {
                foreach (var existingPhoto in trip.Photos)
                {
                    existingPhoto.IsCover = false;
                }
            }

            trip.Photos.Add(photo);
            await _context.SaveChangesAsync();
            return photo;
        }

        public async Task<TripPhoto?> DeletePhotoAsync(int tripId, int photoId)
        {
            var photo = await _context.TripPhotos
                                      .FirstOrDefaultAsync(p => p.TripId == tripId && p.Id == photoId);

            if (photo == null)
                return null;

            _context.TripPhotos.Remove(photo);
            await _context.SaveChangesAsync();
            return photo;
        }

        public override async Task<Trip> UpdateAsync(Trip trip)
        {
            var existingTrip = await _context.Trips
                                             .Include(t => t.Waypoints)
                                             .Include(t => t.Photos)
                                             .FirstOrDefaultAsync(t => t.TripID == trip.TripID);

            if (existingTrip == null)
                throw new KeyNotFoundException($"Trip with id {trip.TripID} not found.");

            _context.Entry(existingTrip).CurrentValues.SetValues(trip);
            _context.TripWaypoints.RemoveRange(existingTrip.Waypoints);
            _context.TripPhotos.RemoveRange(existingTrip.Photos);
            existingTrip.Waypoints = trip.Waypoints;
            existingTrip.Photos = trip.Photos;

            await _context.SaveChangesAsync();
            return existingTrip;
        }
    }
}
