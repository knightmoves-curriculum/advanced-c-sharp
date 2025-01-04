
using Microsoft.AspNetCore.Authorization.Infrastructure;
using MyFirstApi.Models;

namespace MyFirstApi.Services
{
    public class CurrentWeatherForecastService
    {
        private HttpClient httpClient;
        private IWriteRepository<int, WeatherForecast> repository;

        public CurrentWeatherForecastService(HttpClient httpClient, IWriteRepository<int, WeatherForecast> repository)
        {
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("YourAppName/1.0");
            this.httpClient = httpClient;
            this.repository = repository;
        }

        public async Task<WeatherForecast> Report()
        {
            var response = await httpClient.GetFromJsonAsync<WeatherApiResponse>("https://api.weather.gov/gridpoints/DMX/73,49/forecast");

            var periods = response?.Properties.Periods;
            var weatherForecasts = (from period in periods
                        where period.Name == "Today"
                        select new WeatherForecast(period.StartTime, period.Temperature, period.ShortForecast))
                        .ToList();
            repository.Save(weatherForecasts[0]);

            return weatherForecasts[0];
        }
    }
}

public class WeatherApiResponse
{
    public required Properties Properties { get; set; }
}

public class Properties
{
    public required List<Period> Periods { get; set; }
}

public class Period
{
    public required string Name { get; set; }
    public DateTime StartTime { get; set; }
    public double Temperature { get; set; }
    public required string ShortForecast { get; set; }
}