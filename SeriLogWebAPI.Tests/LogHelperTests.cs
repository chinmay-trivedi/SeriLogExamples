using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using SeriLogWebAPI;

namespace SeriLogWebAPI.Tests;

public class LogHelperTests
{
    private sealed class FakeDiagnosticContext : IDiagnosticContext
    {
        public Dictionary<string, object?> Properties { get; } = new();

        public void Set(string propertyName, object value, bool destructureObjects = false) =>
            Properties[propertyName] = value;

        public void SetException(Exception exception) { }
    }

    private static DefaultHttpContext CreateContext(int statusCode = 200)
    {
        var context = new DefaultHttpContext();
        context.Response.StatusCode = statusCode;
        return context;
    }

    private static DefaultHttpContext WithEndpoint(DefaultHttpContext context, string displayName)
    {
        context.SetEndpoint(new Endpoint(_ => Task.CompletedTask, new EndpointMetadataCollection(), displayName));
        return context;
    }

    [Fact]
    public void CustomGetLevel_ReturnsError_WhenExceptionIsPresent() =>
        Assert.Equal(LogEventLevel.Error, LogHelper.CustomGetLevel(CreateContext(), 0, new Exception()));

    [Fact]
    public void CustomGetLevel_ReturnsError_ForServerErrorStatus() =>
        Assert.Equal(LogEventLevel.Error, LogHelper.CustomGetLevel(CreateContext(500), 0, null!));

    [Fact]
    public void CustomGetLevel_ReturnsDebug_ForSuccessfulRequest() =>
        Assert.Equal(LogEventLevel.Debug, LogHelper.CustomGetLevel(CreateContext(200), 0, null!));

    [Fact]
    public void GetLevel_Throws_WhenNoEndpointNamesGiven() =>
        Assert.Throws<ArgumentNullException>(() => LogHelper.GetLevel(LogEventLevel.Verbose));

    [Fact]
    public void GetLevel_UsesTraceLevel_ForMatchingEndpointIgnoringCase()
    {
        var getLevel = LogHelper.GetLevel(LogEventLevel.Verbose, "Health checks");
        var context = WithEndpoint(CreateContext(), "health CHECKS");

        Assert.Equal(LogEventLevel.Verbose, getLevel(context, 0, null!));
    }

    [Fact]
    public void GetLevel_UsesInformation_ForOtherEndpoints()
    {
        var getLevel = LogHelper.GetLevel(LogEventLevel.Verbose, "Health checks");
        var context = WithEndpoint(CreateContext(), "GetWeatherForecast");

        Assert.Equal(LogEventLevel.Information, getLevel(context, 0, null!));
    }

    [Fact]
    public void GetLevel_ReturnsError_EvenForTraceEndpoint_WhenRequestFailed()
    {
        var getLevel = LogHelper.GetLevel(LogEventLevel.Verbose, "Health checks");
        var context = WithEndpoint(CreateContext(503), "Health checks");

        Assert.Equal(LogEventLevel.Error, getLevel(context, 0, null!));
    }

    [Fact]
    public void EnrichFromRequest_SetsRequestProperties_AndQueryStringWhenPresent()
    {
        var diagnosticContext = new FakeDiagnosticContext();
        var context = WithEndpoint(CreateContext(), "GetWeatherForecast");
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.QueryString = new QueryString("?a=1");

        LogHelper.EnrichFromRequest(diagnosticContext, context);

        Assert.Equal("https", diagnosticContext.Properties["Scheme"]);
        Assert.Equal(new HostString("example.com"), diagnosticContext.Properties["Host"]);
        Assert.Equal("?a=1", diagnosticContext.Properties["QueryString"]);
        Assert.Equal("GetWeatherForecast", diagnosticContext.Properties["EndpointName"]);
    }

    [Fact]
    public void EnrichFromRequest_OmitsQueryStringAndEndpoint_WhenAbsent()
    {
        var diagnosticContext = new FakeDiagnosticContext();

        LogHelper.EnrichFromRequest(diagnosticContext, CreateContext());

        Assert.DoesNotContain("QueryString", diagnosticContext.Properties.Keys);
        Assert.DoesNotContain("EndpointName", diagnosticContext.Properties.Keys);
    }
}
