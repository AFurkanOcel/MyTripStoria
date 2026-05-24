namespace Contracts.TripDtos
{
    public class TripMapMarkerDto
    {
        public int TripID { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string MarkerType { get; set; } = string.Empty;
        public string MarkerColor { get; set; } = string.Empty;
        public string? PlaceName { get; set; }
        public string? CityName { get; set; }
        public string? CountryName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
