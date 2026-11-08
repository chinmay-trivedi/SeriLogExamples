using Microsoft.AspNetCore.Http;
using Serilog;
using SeriLogWebAPI;

namespace SeriLogWebAPI.Tests;

public class LogContextEnrichmentTests
{
    private sealed class FakeDiagnosticContext : IDiagnosticContext
    {
        public Dictionary<string, object?> Properties { get; } = new();

        public void Set(string propertyName, object value, bool destructureObjects = false) =>
            Properties[propertyName] = value;

        public void SetException(Exception exception) { }
    }

    [Fact]
    public async Task Invoke_SetsApplicationProperties_AndCallsNext()
    {
        var nextCalled = false;
        var middleware = new LogContextEnrichment(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });
        var diagnosticContext = new FakeDiagnosticContext();

        await middleware.Invoke(new DefaultHttpContext(), diagnosticContext);

        Assert.True(nextCalled);
        Assert.Equal("SeriLogWebAPI", diagnosticContext.Properties["ApplicationName"]);
        Assert.Contains("ApplicationVersion", diagnosticContext.Properties.Keys);
    }
}
