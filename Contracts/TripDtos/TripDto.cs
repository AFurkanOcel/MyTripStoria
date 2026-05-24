namespace Contracts.TripDtos
{
    public class TripDto
    {
        public int TripID { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TripType { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Visibility { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public string? CountryName { get; set; }
        public string? CityName { get; set; }
        public string? PlaceName { get; set; }
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? CoverImageUrl { get; set; }
        public int? Rating { get; set; }
        public decimal? PlannedBudget { get; set; }
        public decimal? ActualCost { get; set; }
        public string? FavoriteMoments { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan TripDuration => EndDate - StartDate;
        public string? Notes { get; set; }
        public List<TripWaypointDto> Waypoints { get; set; } = new();
    }
}
