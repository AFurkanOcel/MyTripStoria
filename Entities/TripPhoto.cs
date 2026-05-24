using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class TripPhoto
    {
        [Key]
        public int Id { get; set; }
        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!;
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;
        [StringLength(200)]
        public string? Caption { get; set; }
        public bool IsCover { get; set; }
        public int SortOrder { get; set; }
        public DateTime? TakenAt { get; set; }
    }
}
