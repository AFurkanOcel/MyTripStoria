using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;

        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<List<Country>> GetAllCountriesAsync()
        {
            return await _countryRepository.GetAllAsync();
        }

        public async Task<Country> GetCountryByIdAsync(int countryId)
        {
            return await _countryRepository.GetByIdAsync(countryId);
        }

        public async Task AddCountryAsync(Country country)
        {
            await _countryRepository.AddAsync(country);
        }

        public async Task<Country> UpdateCountryAsync(Country country)
        {
            return await _countryRepository.UpdateAsync(country);
        }

        public async Task DeleteCountryAsync(int countryId)
        {
            await _countryRepository.DeleteAsync(countryId);
        }
    }
}

