using MyFirstApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IRepository<int, WeatherForecast>, WeatherForecastRepository>();

builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();

app.Run();