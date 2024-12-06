using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using MyFirstApi.Controllers;
using MyFirstApi.Models;

namespace MyFirstApi
{
    public class ApplicationFactory : IControllerFactory
    {
        private static WeatherForecastRepository repository;

        public object CreateController(ControllerContext context)
        {
            Console.WriteLine("setting up controller...");

            if(repository == null){
                Console.WriteLine("setting up singleton repository");
                repository = new WeatherForecastRepository();
            }

            return new WeatherForecastController(repository);
        }

        public void ReleaseController(ControllerContext context, object controller)
        {
            if (controller is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}