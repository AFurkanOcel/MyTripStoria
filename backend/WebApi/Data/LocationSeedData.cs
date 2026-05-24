using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;

namespace WebApi.Data
{
    public static class LocationSeedData
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (await context.Countries.AnyAsync())
                return;

            var countries = new[]
            {
                Country("Türkiye", 38.963745m, 35.243322m, City("İstanbul", 41.008238m, 28.978359m), City("Antalya", 36.896891m, 30.713323m), City("İzmir", 38.423735m, 27.142826m), City("Muğla", 37.215374m, 28.363394m), City("Nevşehir", 38.624420m, 34.723969m)),
                Country("İtalya", 41.871940m, 12.567380m, City("Roma", 41.902782m, 12.496366m), City("Venedik", 45.440847m, 12.315515m), City("Floransa", 43.769562m, 11.255814m), City("Milano", 45.464204m, 9.189982m)),
                Country("Fransa", 46.227638m, 2.213749m, City("Paris", 48.856613m, 2.352222m), City("Nice", 43.710173m, 7.261953m), City("Lyon", 45.764043m, 4.835659m)),
                Country("İspanya", 40.463667m, -3.749220m, City("Barselona", 41.387397m, 2.168568m), City("Madrid", 40.416775m, -3.703790m), City("Sevilla", 37.389092m, -5.984459m)),
                Country("Yunanistan", 39.074208m, 21.824312m, City("Atina", 37.983810m, 23.727539m), City("Santorini", 36.393156m, 25.461509m), City("Selanik", 40.640063m, 22.944419m)),
                Country("Almanya", 51.165691m, 10.451526m, City("Berlin", 52.520008m, 13.404954m), City("Münih", 48.135125m, 11.581981m), City("Hamburg", 53.551086m, 9.993682m)),
                Country("Birleşik Krallık", 55.378051m, -3.435973m, City("Londra", 51.507351m, -0.127758m), City("Edinburgh", 55.953251m, -3.188267m), City("Manchester", 53.480759m, -2.242631m)),
                Country("Amerika Birleşik Devletleri", 37.090240m, -95.712891m, City("New York", 40.712776m, -74.005974m), City("Los Angeles", 34.052235m, -118.243683m), City("Miami", 25.761681m, -80.191788m), City("San Francisco", 37.774929m, -122.419418m)),
                Country("Japonya", 36.204824m, 138.252924m, City("Tokyo", 35.676192m, 139.650311m), City("Kyoto", 35.011636m, 135.768029m), City("Osaka", 34.693738m, 135.502165m)),
                Country("Tayland", 15.870032m, 100.992541m, City("Bangkok", 13.756331m, 100.501762m), City("Phuket", 7.880448m, 98.392250m), City("Chiang Mai", 18.788344m, 98.985300m)),
                Country("Hollanda", 52.132633m, 5.291266m, City("Amsterdam", 52.367573m, 4.904139m), City("Rotterdam", 51.924420m, 4.477733m)),
                Country("Çekya", 49.817492m, 15.472962m, City("Prag", 50.075539m, 14.437800m), City("Brno", 49.195061m, 16.606837m))
            };

            context.Countries.AddRange(countries);
            await context.SaveChangesAsync();
        }

        private static Country Country(string name, decimal latitude, decimal longitude, params City[] cities)
        {
            return new Country
            {
                Name = name,
                Latitude = latitude,
                Longitude = longitude,
                Cities = cities
            };
        }

        private static City City(string name, decimal latitude, decimal longitude)
        {
            return new City
            {
                Name = name,
                Latitude = latitude,
                Longitude = longitude
            };
        }
    }
}
