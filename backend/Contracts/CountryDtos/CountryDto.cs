using Contracts.CityDtos;

namespace Contracts.CountryDtos
{
    public class CountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public ICollection<CityForCountryDto> Cities { get; set; } = new List<CityForCountryDto>();
    }
}
