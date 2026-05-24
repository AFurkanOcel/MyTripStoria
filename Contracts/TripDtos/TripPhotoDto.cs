using System.ComponentModel.DataAnnotations;

namespace Contracts.TripDtos
{
    public class TripPhotoDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;
        [StringLength(200)]
        public string? Caption { get; set; }
        public bool IsCover { get; set; }
        public int SortOrder { get; set; }
        public DateTime? TakenAt { get; set; }
    }
}
