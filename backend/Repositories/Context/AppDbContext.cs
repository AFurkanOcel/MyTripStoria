using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripWaypoint> TripWaypoints { get; set; }
        public DbSet<TripPhoto> TripPhotos { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User ↔ Trip (1:N)
            modelBuilder.Entity<User>()
                        .HasMany(u => u.Trips)
                        .WithOne(t => t.User)
                        .HasForeignKey(t => t.UserID);

            modelBuilder.Entity<User>()
                        .HasIndex(u => u.IdentityUserId)
                        .IsUnique()
                        .HasFilter("[IdentityUserId] IS NOT NULL");

            modelBuilder.Entity<User>()
                        .Property(u => u.CreatedAt)
                        .HasDefaultValueSql("GETUTCDATE()");

            // Country ↔ City (1:N)
            modelBuilder.Entity<Country>()
                        .HasMany(c => c.Cities)
                        .WithOne(c => c.Country)
                        .HasForeignKey(c => c.CountryId);

            // Trip ↔ Country and City (FK)
            modelBuilder.Entity<Trip>()
                        .HasOne(t => t.Country)
                        .WithMany()
                        .HasForeignKey(t => t.CountryId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trip>()
                        .HasOne(t => t.City)
                        .WithMany()
                        .HasForeignKey(t => t.CityId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trip>()
                        .HasMany(t => t.Waypoints)
                        .WithOne(w => w.Trip)
                        .HasForeignKey(w => w.TripId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Trip>()
                        .HasMany(t => t.Photos)
                        .WithOne(p => p.Trip)
                        .HasForeignKey(p => p.TripId)
                        .OnDelete(DeleteBehavior.Cascade);

            // User ↔ Country and City (FK)
            modelBuilder.Entity<User>()
                        .HasOne(u => u.Country)
                        .WithMany()
                        .HasForeignKey(u => u.CountryId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                        .HasOne(u => u.City)
                        .WithMany()
                        .HasForeignKey(u => u.CityId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trip>()
                        .Property(t => t.Status)
                        .HasConversion<string>()
                        .HasMaxLength(20)
                        .HasDefaultValue(TripStatus.Planned);

            modelBuilder.Entity<Trip>()
                        .Property(t => t.Visibility)
                        .HasConversion<string>()
                        .HasMaxLength(20)
                        .HasDefaultValue(TripVisibility.Private);

            modelBuilder.Entity<Trip>()
                        .Property(t => t.CreatedAt)
                        .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Country>()
                        .Property(c => c.Latitude)
                        .HasColumnType("decimal(9,6)");

            modelBuilder.Entity<Country>()
                        .Property(c => c.Longitude)
                        .HasColumnType("decimal(9,6)");

            modelBuilder.Entity<City>()
                        .Property(c => c.Latitude)
                        .HasColumnType("decimal(9,6)");

            modelBuilder.Entity<City>()
                        .Property(c => c.Longitude)
                        .HasColumnType("decimal(9,6)");
        }
    }
}
