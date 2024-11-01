namespace MyFirstApi.Models
{
    public class WeatherForecastRepository : IRepository<int, WeatherForecast>
    {
        private List<WeatherForecast> forecast;

        public WeatherForecastRepository()
        {
            Console.WriteLine("contructing WeatherForecastRepository...");
            forecast = new List<WeatherForecast>();
        }

        public List<WeatherForecast> FindAll()
        {
            return forecast;
        }

        public WeatherForecast FindById(int id)
        {
            return forecast[id];
        }

        public WeatherForecast RemoveById(int id)
        {
            var weatherForecast = forecast[id];
            forecast.Remove(weatherForecast);
            return weatherForecast;
        }

        public WeatherForecast Save(WeatherForecast weatherForecast)
        {
            forecast.Add(weatherForecast);
            return weatherForecast;
        }

        public WeatherForecast Update(int id, WeatherForecast weatherForecast)
        {
            forecast[id] = weatherForecast;
            return weatherForecast;
        }
    }
}