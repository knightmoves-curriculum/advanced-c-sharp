using MyFirstApi.Models;

namespace MyFirstApi.Services
{
    public class CityForecastService
    {
        private IReadRepository<int, City> cityRepository;
        private IWriteRepository<int, CityWeatherForecast> cityWeatherForecastRepository;

        public CityForecastService(IReadRepository<int, City> cityRepository, IWriteRepository<int, CityWeatherForecast> cityWeatherForecastRepository)
        {
            this.cityRepository = cityRepository;
            this.cityWeatherForecastRepository = cityWeatherForecastRepository;
        }

        public  WeatherForecast Associate(WeatherForecast weatherForecast, ICollection<int> cityIds)
        {
            if(cityIds != null)
            {
                foreach (int cityId in cityIds)
                {
                    City city = cityRepository.FindById(cityId);
                    CityWeatherForecast cityWeatherForecast = new();
                    cityWeatherForecast.CityId = cityId;
                    cityWeatherForecast.City = city;
                    cityWeatherForecast.WeatherForecastId = weatherForecast.Id;
                    cityWeatherForecast.WeatherForecast = weatherForecast;

                    cityWeatherForecastRepository.Save(cityWeatherForecast);
                }
            }
            
            return weatherForecast;
        }
    }
}