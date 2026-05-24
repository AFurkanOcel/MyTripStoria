namespace Contracts.CountryDtos
{
    public class CountryPutDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
