using Microsoft.AspNetCore.Mvc;
using Serilog;
namespace SeriLogWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly Serilog.ILogger _myLogger = Log.ForContext<WeatherForecastController>();

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;            
            _logger.LogInformation("WeatherForecast controller called ");
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("WeatherForecast get method Starting.");
            _logger.LogDebug("WeatherForecast get method Starting.");
            _logger.LogWarning("WeatherForecast get method Starting.");
            _logger.LogCritical("WeatherForecast get method Starting.");
            var myClassLogger = _myLogger.ForContext("SourceContext", "MyClass");
            myClassLogger.Information("MyClass Logging is enabled !!!");
            myClassLogger.Debug("MyClass Debug Logging is enabled !!!");
            myClassLogger.Error("MyClass Error Logging is enabled !!!");
            myClassLogger.Fatal("MyClass Fatal Logging is enabled !!!");


            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }


    }
}
