using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class TripWaypoint
    {
        [Key]
        public int Id { get; set; }
        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!;
        [StringLength(120)]
        public string Name { get; set; } = string.Empty;
        [StringLength(300)]
        public string? Address { get; set; }
        [Column(TypeName = "decimal(9,6)")]
        public decimal Latitude { get; set; }
        [Column(TypeName = "decimal(9,6)")]
        public decimal Longitude { get; set; }
        public int SortOrder { get; set; }
        public DateTime? PlannedAt { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}
