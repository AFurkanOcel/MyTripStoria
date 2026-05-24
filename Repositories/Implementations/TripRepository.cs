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
