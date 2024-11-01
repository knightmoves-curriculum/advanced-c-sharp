using Microsoft.AspNetCore.Mvc.Controllers;
using MyFirstApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IControllerFactory, ControllerFactory>();

builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();

app.Run();