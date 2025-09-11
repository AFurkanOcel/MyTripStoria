using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.CityDtos;
using Entities;

namespace Contracts.CountryDtos
{
    public class CountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CityForCountryDto> Cities { get; set; } = new List<CityForCountryDto>();
    }
}
