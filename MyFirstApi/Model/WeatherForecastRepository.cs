namespace MyFirstApi.Models
{
    public class WeatherForecastRepository : IWriteRepository<int, WeatherForecast>, IReadRepository<int, WeatherForecast>
    {
        private WeatherForecastDbContext context;

        public WeatherForecastRepository(WeatherForecastDbContext context)
        {
            this.context = context;
        }

        public WeatherForecast Save(WeatherForecast weatherForecast)
        {
            context.WeatherForecasts.Add(weatherForecast);
            context.SaveChanges();
            return weatherForecast;
        }

        public WeatherForecast Update(int id, WeatherForecast weatherForecast)
        {
            weatherForecast.Id = id;
            context.WeatherForecasts.Update(weatherForecast);
            context.SaveChanges();
            return weatherForecast;
        }

        public List<WeatherForecast> FindAll()
        {
            return context.WeatherForecasts.ToList();
        }

        public WeatherForecast FindById(int id)
        {
            return context.WeatherForecasts.Find(id);
        }

        public WeatherForecast RemoveById(int id)
        {
            var weatherForecast = context.WeatherForecasts.Find(id);
            context.WeatherForecasts.Remove(weatherForecast);
            context.SaveChanges();
            return weatherForecast;
        }

        public int Count()
        {
            return context.WeatherForecasts.Count();
        }
    }
}