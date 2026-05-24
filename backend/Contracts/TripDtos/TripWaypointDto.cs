using System.ComponentModel.DataAnnotations;

namespace Contracts.TripDtos
{
    public class TripWaypointDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Name { get; set; } = string.Empty;
        [StringLength(300)]
        public string? Address { get; set; }
        [Range(-90, 90)]
        public decimal Latitude { get; set; }
        [Range(-180, 180)]
        public decimal Longitude { get; set; }
        public int SortOrder { get; set; }
        public DateTime? PlannedAt { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}
