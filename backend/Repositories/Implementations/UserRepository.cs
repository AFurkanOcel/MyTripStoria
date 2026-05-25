using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                                 .Include(u => u.Trips)
                                 .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                                 .Include(u => u.Trips)
                                 .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdentityUserIdAsync(string identityUserId)
        {
            return await _context.Users
                                 .Include(u => u.Trips)
                                 .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);
        }

        public override async Task<User> UpdateAsync(User user)
        {
            var existingUser = await _context.Users
                                             .FirstOrDefaultAsync(u => u.UserID == user.UserID);

            if (existingUser == null)
                throw new KeyNotFoundException($"User with id {user.UserID} not found.");

            _context.Entry(existingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
            return existingUser;
        }
    }
}

