using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Repositories.Interfaces
{
    public interface ICityRepository : IRepository<City>
    {
        Task<List<City>> GetAllByCountryIdAsync(int countryId);
        Task<City?> GetByNameAsync(string name);
    }
}
