using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class Trip
    {
        [Key]
        public int TripID { get; set; }
        public int UserID { get; set; }
        public User User { get; set; } = null!;
        public bool IsCompleted { get; set; }
        public TripStatus Status { get; set; } = TripStatus.Planned;
        public TripVisibility Visibility { get; set; } = TripVisibility.Private;
        public string TripType { get; set; } = string.Empty;
        [StringLength(120)]
        public string Title { get; set; } = string.Empty;
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;
        public int CityId { get; set; }
        public City City { get; set; } = null!;
        [StringLength(120)]
        public string? PlaceName { get; set; }
        [StringLength(300)]
        public string? Address { get; set; }
        [Column(TypeName = "decimal(9,6)")]
        public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(9,6)")]
        public decimal? Longitude { get; set; }
        [StringLength(500)]
        public string? CoverImageUrl { get; set; }
        [Range(1, 5)]
        public int? Rating { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PlannedBudget { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ActualCost { get; set; }
        [StringLength(1000)]
        public string? FavoriteMoments { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan TripDuration => EndDate - StartDate;
        [StringLength(1000)]
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<TripWaypoint> Waypoints { get; set; } = new List<TripWaypoint>();
        public ICollection<TripPhoto> Photos { get; set; } = new List<TripPhoto>();
    }
}
