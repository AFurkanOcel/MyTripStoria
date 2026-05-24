namespace Contracts.UserDtos
{
    public class UserPostDto
    {
        public string? IdentityUserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int Age { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public string? Address { get; set; }
        public decimal Budget { get; set; }
        public bool IsPremium { get; set; }
    }
}
