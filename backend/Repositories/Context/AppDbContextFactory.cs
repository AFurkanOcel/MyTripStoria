using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Repositories.Context
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = Environment.GetEnvironmentVariable("MYTRIPSTORIA_CONNECTION_STRING")
                                   ?? "Server=(localdb)\\MyTripStoriaLocalDb;Database=MyTripStoriaDb;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true";

            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}

