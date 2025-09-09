using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Services.Interfaces
{
    public interface ICityService
    {
        Task<List<City>> GetAllCitiesAsync();
        Task<City> GetCityByIdAsync(int cityId);
        Task AddCityAsync(City city);
        Task<City> UpdateCityAsync(City city);
        Task DeleteCityAsync(int cityId);
    }
}
