using System.ComponentModel.DataAnnotations;

namespace Contracts.TripDtos
{
    public class TripPutDto
    {
        [Required]
        public int UserId { get; set; }
        public bool IsCompleted { get; set; }
        [Required]
        [StringLength(120)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string TripType { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public string? Status { get; set; }
        public string? Visibility { get; set; }
        [StringLength(120)]
        public string? PlaceName { get; set; }
        [StringLength(300)]
        public string? Address { get; set; }
        [Range(-90, 90)]
        public decimal? Latitude { get; set; }
        [Range(-180, 180)]
        public decimal? Longitude { get; set; }
        [StringLength(500)]
        public string? CoverImageUrl { get; set; }
        [Range(1, 5)]
        public int? Rating { get; set; }
        [Range(0, 999999999)]
        public decimal? PlannedBudget { get; set; }
        [Range(0, 999999999)]
        public decimal? ActualCost { get; set; }
        [StringLength(1000)]
        public string? FavoriteMoments { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        public List<TripWaypointDto> Waypoints { get; set; } = new();
        public List<TripPhotoDto> Photos { get; set; } = new();
    }
}
