using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<List<Trip>> GetAllTripsAsync()
        {
            return await _tripRepository.GetAllAsync();
        }

        public async Task<Trip> GetTripByIdAsync(int tripId)
        {
            return await _tripRepository.GetByIdAsync(tripId);
        }

        public async Task<List<Trip>> GetTripsByUserIdAsync(int userId)
        {
            return await _tripRepository.GetAllByUserIdAsync(userId);
        }

        public async Task AddTripAsync(Trip trip)
        {
            await _tripRepository.AddAsync(trip);
        }

        public async Task DeleteTripAsync(int tripId)
        {
            await _tripRepository.DeleteAsync(tripId);
        }

        public async Task<Trip> UpdateTripAsync(Trip trip)
        {
            return await _tripRepository.UpdateAsync(trip);
        }
    }
}
