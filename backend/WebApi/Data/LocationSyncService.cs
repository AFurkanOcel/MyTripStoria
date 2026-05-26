using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;

namespace WebApi.Data
{
    public sealed class LocationSyncService
    {
        private static readonly Dictionary<string, string[]> CountryAliases = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Turkey"] = ["Turkey", "Turkiye"],
            ["Italy"] = ["Italy"],
            ["France"] = ["France"],
            ["Spain"] = ["Spain"],
            ["Greece"] = ["Greece"],
            ["Germany"] = ["Germany"],
            ["United Kingdom"] = ["United Kingdom", "UK", "Great Britain"],
            ["United States"] = ["United States", "United States of America", "USA"],
            ["Japan"] = ["Japan"],
            ["Thailand"] = ["Thailand"],
            ["Netherlands"] = ["Netherlands"],
            ["Czechia"] = ["Czechia", "Czech Republic"],
            ["Hungary"] = ["Hungary"]
        };

        private static readonly Dictionary<string, string[]> PreferredCities = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Turkey"] = ["Istanbul", "Antalya", "Izmir", "Mugla", "Nevsehir"],
            ["Italy"] = ["Rome", "Venice", "Florence", "Milan"],
            ["France"] = ["Paris", "Nice", "Lyon"],
            ["Spain"] = ["Barcelona", "Madrid", "Seville"],
            ["Greece"] = ["Athens", "Santorini", "Thessaloniki"],
            ["Germany"] = ["Berlin", "Munich", "Hamburg"],
            ["United Kingdom"] = ["London", "Edinburgh", "Manchester"],
            ["United States"] = ["New York", "Los Angeles", "Miami", "San Francisco"],
            ["Japan"] = ["Tokyo", "Kyoto", "Osaka"],
            ["Thailand"] = ["Bangkok", "Phuket", "Chiang Mai"],
            ["Netherlands"] = ["Amsterdam", "Rotterdam"],
            ["Czechia"] = ["Prague", "Brno"],
            ["Hungary"] = ["Budapest"]
        };

        private static readonly Dictionary<string, (decimal Latitude, decimal Longitude)> CityCoordinates = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Adana"] = (37.000000m, 35.321333m),
            ["Ankara"] = (39.933365m, 32.859742m),
            ["Istanbul"] = (41.008238m, 28.978359m),
            ["Antalya"] = (36.896891m, 30.713323m),
            ["Bursa"] = (40.188528m, 29.060964m),
            ["Eskisehir"] = (39.776667m, 30.520556m),
            ["Gaziantep"] = (37.066220m, 37.383320m),
            ["Izmir"] = (38.423735m, 27.142826m),
            ["Kayseri"] = (38.720489m, 35.482597m),
            ["Konya"] = (37.874641m, 32.493156m),
            ["Mugla"] = (37.215374m, 28.363394m),
            ["Nevsehir"] = (38.624420m, 34.723969m),
            ["Samsun"] = (41.286667m, 36.33m),
            ["Trabzon"] = (41.002697m, 39.716763m),
            ["Rome"] = (41.902782m, 12.496366m),
            ["Venice"] = (45.440847m, 12.315515m),
            ["Florence"] = (43.769562m, 11.255814m),
            ["Milan"] = (45.464204m, 9.189982m),
            ["Paris"] = (48.856613m, 2.352222m),
            ["Nice"] = (43.710173m, 7.261953m),
            ["Lyon"] = (45.764043m, 4.835659m),
            ["Barcelona"] = (41.387397m, 2.168568m),
            ["Madrid"] = (40.416775m, -3.703790m),
            ["Seville"] = (37.389092m, -5.984459m),
            ["Athens"] = (37.983810m, 23.727539m),
            ["Santorini"] = (36.393156m, 25.461509m),
            ["Thessaloniki"] = (40.640063m, 22.944419m),
            ["Berlin"] = (52.520008m, 13.404954m),
            ["Munich"] = (48.135125m, 11.581981m),
            ["Hamburg"] = (53.551086m, 9.993682m),
            ["London"] = (51.507351m, -0.127758m),
            ["Edinburgh"] = (55.953251m, -3.188267m),
            ["Manchester"] = (53.480759m, -2.242631m),
            ["New York"] = (40.712776m, -74.005974m),
            ["Los Angeles"] = (34.052235m, -118.243683m),
            ["Miami"] = (25.761681m, -80.191788m),
            ["San Francisco"] = (37.774929m, -122.419418m),
            ["Tokyo"] = (35.676192m, 139.650311m),
            ["Kyoto"] = (35.011636m, 135.768029m),
            ["Osaka"] = (34.693738m, 135.502165m),
            ["Bangkok"] = (13.756331m, 100.501762m),
            ["Phuket"] = (7.880448m, 98.392250m),
            ["Chiang Mai"] = (18.788344m, 98.985300m),
            ["Amsterdam"] = (52.367573m, 4.904139m),
            ["Rotterdam"] = (51.924420m, 4.477733m),
            ["Prague"] = (50.075539m, 14.437800m),
            ["Brno"] = (49.195061m, 16.606837m),
            ["Budapest"] = (47.497913m, 19.040236m)
        };

        private static readonly Dictionary<string, string> LegacyCountryNames = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Türkiye"] = "Turkey",
            ["TÃ¼rkiye"] = "Turkey",
            ["İtalya"] = "Italy",
            ["Ä°talya"] = "Italy",
            ["Fransa"] = "France",
            ["İspanya"] = "Spain",
            ["Ä°spanya"] = "Spain",
            ["Yunanistan"] = "Greece",
            ["Almanya"] = "Germany",
            ["Birleşik Krallık"] = "United Kingdom",
            ["BirleÅŸik KrallÄ±k"] = "United Kingdom",
            ["Amerika Birleşik Devletleri"] = "United States",
            ["Amerika BirleÅŸik Devletleri"] = "United States",
            ["Japonya"] = "Japan",
            ["Tayland"] = "Thailand",
            ["Hollanda"] = "Netherlands",
            ["Çekya"] = "Czechia",
            ["Ã‡ekya"] = "Czechia"
        };

        private static readonly Dictionary<string, string> LegacyCityNames = new(StringComparer.OrdinalIgnoreCase)
        {
            ["İstanbul"] = "Istanbul",
            ["Ä°stanbul"] = "Istanbul",
            ["İzmir"] = "Izmir",
            ["Ä°zmir"] = "Izmir",
            ["Muğla"] = "Mugla",
            ["MuÄŸla"] = "Mugla",
            ["Nevşehir"] = "Nevsehir",
            ["NevÅŸehir"] = "Nevsehir",
            ["Roma"] = "Rome",
            ["Venedik"] = "Venice",
            ["Floransa"] = "Florence",
            ["Milano"] = "Milan",
            ["Barselona"] = "Barcelona",
            ["Sevilla"] = "Seville",
            ["Atina"] = "Athens",
            ["Selanik"] = "Thessaloniki",
            ["Münih"] = "Munich",
            ["MÃ¼nih"] = "Munich",
            ["Londra"] = "London",
            ["Prag"] = "Prague"
        };

        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly ILogger<LocationSyncService> _logger;

        public LocationSyncService(AppDbContext context, HttpClient httpClient, ILogger<LocationSyncService> logger)
        {
            _context = context;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task SyncAsync()
        {
            NormalizeLegacyLocationNames();

            var catalog = await BuildExternalCatalogAsync();
            if (catalog.Count == 0)
            {
                _logger.LogWarning("External location APIs did not return data. Falling back to the bundled English travel catalog.");
                catalog = BuildFallbackCatalog();
            }

            await UpsertCatalogAsync(catalog);
            await _context.SaveChangesAsync();
        }

        private async Task<List<LocationCountry>> BuildExternalCatalogAsync()
        {
            try
            {
                var restCountries = await _httpClient.GetFromJsonAsync<List<RestCountry>>(
                    "https://restcountries.com/v3.1/independent?status=true&fields=name,latlng,capital");

                if (restCountries == null || restCountries.Count == 0)
                    return [];

                var countriesNowCatalog = await GetCountriesNowCatalogAsync();
                var countries = new List<LocationCountry>();
                foreach (var restCountry in restCountries.OrderBy(country => country.Name.Common))
                {
                    var name = restCountry.Name.Common;
                    var countryLatitude = ToDecimal(restCountry.Latlng.ElementAtOrDefault(0));
                    var countryLongitude = ToDecimal(restCountry.Latlng.ElementAtOrDefault(1));

                    var cities = FindCitiesFromCountriesNow(countriesNowCatalog, name);
                    if (cities.Count == 0 && PreferredCities.TryGetValue(name, out var preferredCities))
                        cities = preferredCities.ToList();
                    if (cities.Count == 0)
                        cities = restCountry.Capital.Where(capital => !string.IsNullOrWhiteSpace(capital)).ToList();
                    if (cities.Count == 0)
                        cities = ["Capital area"];

                    countries.Add(new LocationCountry(
                        name,
                        countryLatitude,
                        countryLongitude,
                        cities
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .OrderBy(city => city)
                            .Select(city => CreateLocationCity(city, countryLatitude, countryLongitude))
                            .ToList()));
                }

                return countries;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not synchronize locations from external APIs.");
                return [];
            }
        }

        private async Task<List<CountriesNowCountryCities>> GetCountriesNowCatalogAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<CountriesNowCountriesResponse>(
                "https://countriesnow.space/api/v0.1/countries");

            return response?.Error == false ? response.Data : [];
        }

        private static List<string> FindCitiesFromCountriesNow(List<CountriesNowCountryCities> catalog, string countryName)
        {
            var aliases = CountryAliases.TryGetValue(countryName, out var values) ? values : [countryName];
            var match = catalog.FirstOrDefault(country =>
                aliases.Any(alias => string.Equals(country.Country, alias, StringComparison.OrdinalIgnoreCase)));

            return match?.Cities
                .Where(city => !string.IsNullOrWhiteSpace(city))
                .ToList() ?? [];
        }

        private async Task<List<string>> GetPreferredCitiesFromApiAsync(string countryName)
        {
            if (!PreferredCities.TryGetValue(countryName, out var preferredCities))
                return [];

            var aliases = CountryAliases.TryGetValue(countryName, out var values) ? values : [countryName];
            foreach (var alias in aliases)
            {
                var response = await _httpClient.PostAsJsonAsync(
                    "https://countriesnow.space/api/v0.1/countries/cities",
                    new { country = alias });

                if (!response.IsSuccessStatusCode)
                    continue;

                var result = await response.Content.ReadFromJsonAsync<CountriesNowCitiesResponse>();
                if (result?.Error != false || result.Data.Count == 0)
                    continue;

                var apiCities = new HashSet<string>(result.Data, StringComparer.OrdinalIgnoreCase);
                var selected = preferredCities.Where(apiCities.Contains).ToList();
                return selected.Count > 0 ? selected : preferredCities.ToList();
            }

            return [];
        }

        private static RestCountry? FindRestCountry(IEnumerable<RestCountry> countries, string preferredName)
        {
            var aliases = CountryAliases.TryGetValue(preferredName, out var values) ? values : [preferredName];
            return countries.FirstOrDefault(country =>
                aliases.Any(alias => string.Equals(country.Name.Common, alias, StringComparison.OrdinalIgnoreCase)));
        }

        private async Task UpsertCatalogAsync(List<LocationCountry> catalog)
        {
            var existingCountries = await _context.Countries
                .Include(country => country.Cities)
                .ToListAsync();

            foreach (var countryData in catalog)
            {
                var country = existingCountries.FirstOrDefault(country =>
                    string.Equals(country.Name, countryData.Name, StringComparison.OrdinalIgnoreCase));

                if (country == null)
                {
                    country = new Country { Name = countryData.Name };
                    _context.Countries.Add(country);
                    existingCountries.Add(country);
                }

                country.Latitude = countryData.Latitude;
                country.Longitude = countryData.Longitude;

                foreach (var cityData in countryData.Cities)
                {
                    var city = country.Cities.FirstOrDefault(existingCity =>
                        string.Equals(existingCity.Name, cityData.Name, StringComparison.OrdinalIgnoreCase));

                    if (city == null)
                    {
                        country.Cities.Add(new City
                        {
                            Name = cityData.Name,
                            Latitude = cityData.Latitude,
                            Longitude = cityData.Longitude
                        });
                    }
                    else
                    {
                        city.Latitude = cityData.Latitude;
                        city.Longitude = cityData.Longitude;
                    }
                }
            }
        }

        private void NormalizeLegacyLocationNames()
        {
            foreach (var country in _context.Countries)
            {
                if (LegacyCountryNames.TryGetValue(country.Name, out var englishName))
                    country.Name = englishName;
            }

            foreach (var city in _context.Cities)
            {
                if (LegacyCityNames.TryGetValue(city.Name, out var englishName))
                    city.Name = englishName;
            }
        }

        private static LocationCity CreateLocationCity(string name)
        {
            var coordinates = CityCoordinates.GetValueOrDefault(name);
            return new LocationCity(name, coordinates.Latitude, coordinates.Longitude);
        }

        private static LocationCity CreateLocationCity(string name, decimal? fallbackLatitude, decimal? fallbackLongitude)
        {
            var coordinates = CityCoordinates.GetValueOrDefault(name);
            var latitude = coordinates.Latitude == 0 ? fallbackLatitude : coordinates.Latitude;
            var longitude = coordinates.Longitude == 0 ? fallbackLongitude : coordinates.Longitude;
            return new LocationCity(name, latitude, longitude);
        }

        private static List<LocationCountry> BuildFallbackCatalog()
        {
            return PreferredCities.Select(country =>
            {
                var countryCoordinates = country.Key switch
                {
                    "Turkey" => (38.963745m, 35.243322m),
                    "Italy" => (41.871940m, 12.567380m),
                    "France" => (46.227638m, 2.213749m),
                    "Spain" => (40.463667m, -3.749220m),
                    "Greece" => (39.074208m, 21.824312m),
                    "Germany" => (51.165691m, 10.451526m),
                    "United Kingdom" => (55.378051m, -3.435973m),
                    "United States" => (37.090240m, -95.712891m),
                    "Japan" => (36.204824m, 138.252924m),
                    "Thailand" => (15.870032m, 100.992541m),
                    "Netherlands" => (52.132633m, 5.291266m),
                    "Czechia" => (49.817492m, 15.472962m),
                    "Hungary" => (47.162494m, 19.503304m),
                    _ => (0m, 0m)
                };

                return new LocationCountry(
                    country.Key,
                    countryCoordinates.Item1,
                    countryCoordinates.Item2,
                    country.Value.Select(CreateLocationCity).ToList());
            }).ToList();
        }

        private static decimal? ToDecimal(double value)
        {
            return value == 0 ? null : Convert.ToDecimal(value);
        }

        private sealed record LocationCountry(string Name, decimal? Latitude, decimal? Longitude, List<LocationCity> Cities);

        private sealed record LocationCity(string Name, decimal? Latitude, decimal? Longitude);

        private sealed class RestCountry
        {
            [JsonPropertyName("name")]
            public RestCountryName Name { get; set; } = new();

            [JsonPropertyName("latlng")]
            public List<double> Latlng { get; set; } = [];

            [JsonPropertyName("capital")]
            public List<string> Capital { get; set; } = [];
        }

        private sealed class RestCountryName
        {
            [JsonPropertyName("common")]
            public string Common { get; set; } = string.Empty;
        }

        private sealed class CountriesNowCitiesResponse
        {
            [JsonPropertyName("error")]
            public bool Error { get; set; }

            [JsonPropertyName("data")]
            public List<string> Data { get; set; } = [];
        }

        private sealed class CountriesNowCountriesResponse
        {
            [JsonPropertyName("error")]
            public bool Error { get; set; }

            [JsonPropertyName("data")]
            public List<CountriesNowCountryCities> Data { get; set; } = [];
        }

        private sealed class CountriesNowCountryCities
        {
            [JsonPropertyName("country")]
            public string Country { get; set; } = string.Empty;

            [JsonPropertyName("cities")]
            public List<string> Cities { get; set; } = [];
        }
    }
}
