using MyFirstApi.Models;
using MyFirstApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<WeatherForecastRepository>();
builder.Services.AddSingleton<IReadRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());
builder.Services.AddSingleton<IWriteRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());

builder.Services.AddTransient<CurrentWeatherForecastService>();
builder.Services.AddHttpClient<CurrentWeatherForecastService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
