using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Repositories.Interfaces
{
    public interface ITripRepository : IRepository<Trip>
    {
        Task<List<Trip>> GetAllByUserIdAsync(int userId);
    }
}
