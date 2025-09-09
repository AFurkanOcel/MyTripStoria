using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class CityRepository : Repository<City>, ICityRepository
    {
        public CityRepository(AppDbContext context) : base(context) { }

        public async Task<List<City>> GetAllByCountryIdAsync(int countryId)
        {
            return await _context.Cities
                                 .Where(c => c.CountryId == countryId)
                                 .ToListAsync();
        }
    }
}
