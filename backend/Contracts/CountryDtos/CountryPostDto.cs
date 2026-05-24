namespace Contracts.CountryDtos
{
    public class CountryPostDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
