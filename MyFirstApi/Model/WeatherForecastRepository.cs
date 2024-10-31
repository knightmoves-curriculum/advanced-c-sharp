namespace MyFirstApi.Models
{
    public class WeatherForecastRepository
    {
        private List<WeatherForecast> forecast;

        public WeatherForecastRepository()
        {
            forecast = new List<WeatherForecast>();
        }

        internal List<WeatherForecast> FindAll()
        {
            return forecast;
        }

        internal WeatherForecast FindById(int id)
        {
            return forecast[id];
        }

        internal WeatherForecast RemoveById(int id)
        {
            var weatherForecast = forecast[id];
            forecast.Remove(weatherForecast);
            return weatherForecast;
        }

        internal void Save(WeatherForecast weatherForecast)
        {
            forecast.Add(weatherForecast);
        }

        internal void Update(int id, WeatherForecast weatherForecast)
        {
            forecast[id] = weatherForecast;
        }
    }
}