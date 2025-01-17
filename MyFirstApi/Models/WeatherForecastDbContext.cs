using Microsoft.EntityFrameworkCore;

namespace MyFirstApi.Models
{
    public class WeatherForecastDbContext : DbContext
    {
        public WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options): base(options) { }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
        public DbSet<WeatherAlert> WeatherAlerts { get; set; }
        public DbSet<WeatherComment> WeatherComments { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<CityWeatherForecast> CityWeatherForecasts { get; set; }
        public DbSet<User> Users { get; set; }
    }
}