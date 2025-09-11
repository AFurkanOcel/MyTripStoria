using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.TripDtos
{
    public enum TripType
    {
        Individual,
        Family,
        Friends,
        Tour
    }

    public class TripDto
    {
        public int TripID { get; set; }
        public bool IsCompleted { get; set; }
        public TripType Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        // Id's for Backend
        public int CountryId { get; set; }
        public int CityId { get; set; }
        // Names for Frontend
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan TripDuration => EndDate - StartDate;
        public string Notes { get; set; }
    }
}
