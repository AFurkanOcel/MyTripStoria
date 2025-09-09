using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Repositories.Interfaces
{
        public interface IUserRepository : IRepository<User>
        {
            Task<User> GetByUsernameAsync(string username);
            Task<User> GetByEmailAsync(string email);
        }
}
