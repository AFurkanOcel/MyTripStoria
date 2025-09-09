using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Services.Interfaces
{
    public interface ICountryService
    {
        Task<List<Country>> GetAllCountriesAsync();
        Task<Country> GetCountryByIdAsync(int countryId);
        Task AddCountryAsync(Country country);
        Task<Country> UpdateCountryAsync(Country country);
        Task DeleteCountryAsync(int countryId);
    }
}
