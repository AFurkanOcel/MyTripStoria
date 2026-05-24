namespace Contracts.CityDtos
{
    public class CityPostDto
    {
        public string Name { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
