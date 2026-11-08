using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SeriLogNet8.ConsoleApp;

namespace SeriLogNet8.Tests;

public class MyServiceTests
{
    [Fact]
    public void DoSomething_LogsOneError()
    {
        var logger = new CapturingLogger<MyService>();
        var service = new MyService(new ConfigurationBuilder().Build(), logger);

        service.DoSomething();

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Equal("Exception Occurred during MyService DoSomething().", entry.Message);
    }
}
