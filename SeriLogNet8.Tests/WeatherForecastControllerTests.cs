using Microsoft.Extensions.Logging;
using SerilogNet8.WebAPI.Controllers;

namespace SeriLogNet8.Tests;

public class WeatherForecastControllerTests
{
    [Fact]
    public void Get_ReturnsFiveForecastsWithinTemperatureRange()
    {
        var controller = new WeatherForecastController(new CapturingLogger<WeatherForecastController>());

        var forecasts = controller.Get().ToList();

        Assert.Equal(5, forecasts.Count);
        Assert.All(forecasts, f =>
        {
            Assert.InRange(f.TemperatureC, -20, 54);
            Assert.False(string.IsNullOrEmpty(f.Summary));
        });
    }

    [Fact]
    public void Get_LogsAtEverySeverity()
    {
        var logger = new CapturingLogger<WeatherForecastController>();

        new WeatherForecastController(logger).Get();

        Assert.Equal(
            new[] { LogLevel.Information, LogLevel.Debug, LogLevel.Warning, LogLevel.Error, LogLevel.Critical },
            logger.Entries.Select(e => e.Level));
    }
}
