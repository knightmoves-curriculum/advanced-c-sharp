namespace MyFirstApi.Models
{
    using Microsoft.EntityFrameworkCore;

    public class CityWeatherForecastRepository : IWriteRepository<int, CityWeatherForecast>, IReadRepository<int, CityWeatherForecast>
    {
        private WeatherForecastDbContext context;

        public CityWeatherForecastRepository(WeatherForecastDbContext context)
        {
            this.context = context;
        }

        public CityWeatherForecast Save(CityWeatherForecast cityWeatherForecast)
        {
            context.CityWeatherForecasts.Add(cityWeatherForecast);
            context.SaveChanges();
            return cityWeatherForecast;
        }

        public CityWeatherForecast Update(int id, CityWeatherForecast cityWeatherForecast)
        {
            cityWeatherForecast.Id = id;
            context.CityWeatherForecasts.Update(cityWeatherForecast);
            context.SaveChanges();
            return cityWeatherForecast;
        }

        public CityWeatherForecast RemoveById(int id)
        {
            var cityWeatherForecast = context.CityWeatherForecasts.Find(id);
            context.CityWeatherForecasts.Remove(cityWeatherForecast);
            context.SaveChanges();
            return cityWeatherForecast;
        }

        public List<CityWeatherForecast> FindAll()
        {
            return context.CityWeatherForecasts
            .ToList();
        }

        public CityWeatherForecast FindById(int id)
        {
            return context.CityWeatherForecasts.Find(id);
        }

        public int Count()
        {
            return context.CityWeatherForecasts.Count();
        }
    }
}