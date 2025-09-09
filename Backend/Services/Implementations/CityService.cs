using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<List<City>> GetAllCitiesAsync()
        {
            return await _cityRepository.GetAllAsync();
        }

        public async Task<City> GetCityByIdAsync(int cityId)
        {
            return await _cityRepository.GetByIdAsync(cityId);
        }

        public async Task AddCityAsync(City city)
        {
            await _cityRepository.AddAsync(city);
        }

        public async Task<City> UpdateCityAsync(City city)
        {
            return await _cityRepository.UpdateAsync(city);
        }

        public async Task DeleteCityAsync(int cityId)
        {
            await _cityRepository.DeleteAsync(cityId);
        }
    }
}
