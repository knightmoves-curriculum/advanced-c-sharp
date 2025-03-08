using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WeatherAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateTemperatureAttribute : ActionFilterAttribute
    {
        private readonly int _minTemp;
        private readonly int _maxTemp;

        public ValidateTemperatureAttribute(int minTemp, int maxTemp)
        {
            _minTemp = minTemp;
            _maxTemp = maxTemp;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.TryGetValue("weatherForecastDto", out var value) &&
                value is WeatherForecastDto weatherForecastDto)
            {
                if (weatherForecastDto.TemperatureF < _minTemp || weatherForecastDto.TemperatureF > _maxTemp)
                {
                    context.Result = new BadRequestObjectResult($"Temperature must be between {_minTemp} and {_maxTemp} degrees.");
                }
            }
        }
    }
}