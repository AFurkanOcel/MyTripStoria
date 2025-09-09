using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class CountryRepository : Repository<Country>, ICountryRepository
    {
        public CountryRepository(AppDbContext context) : base(context) { }

        public async Task<Country> GetByNameAsync(string name)
        {
            return await _context.Countries
                                 .Include(c => c.Cities)
                                 .FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}

