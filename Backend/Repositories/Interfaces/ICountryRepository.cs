using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Repositories.Interfaces
{
    public interface ICountryRepository : IRepository<Country>
    {
        Task<List<Country?>> GetAllCountryAsync();
        Task<Country?> GetCountryByIdAsync(int id);
    }
}
