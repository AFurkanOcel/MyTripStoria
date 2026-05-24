using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        [StringLength(450)]
        public string? IdentityUserId { get; set; }
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;
        public int Age { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;
        public int CityId { get; set; }
        public City City { get; set; } = null!;
        [StringLength(200)]
        public string? Address { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Budget { get; set; }
        public bool IsPremium { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
