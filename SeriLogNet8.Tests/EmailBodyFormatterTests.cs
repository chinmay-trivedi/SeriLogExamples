using Serilog.Events;
using Serilog.Parsing;
using SeriLogNet8.ConsoleApp;

namespace SeriLogNet8.Tests;

public class EmailBodyFormatterTests
{
    private static LogEvent CreateEvent(string template, params LogEventProperty[] properties) =>
        new(DateTimeOffset.UtcNow, LogEventLevel.Information, null,
            new MessageTemplateParser().Parse(template), properties);

    [Fact]
    public void Format_HtmlEncodesRenderedMessage()
    {
        var writer = new StringWriter();

        new EmailBodyFormatter().Format(CreateEvent("<b>bold</b> & more"), writer);

        Assert.Equal("&lt;b&gt;bold&lt;/b&gt; &amp; more", writer.ToString());
    }

    [Fact]
    public void Format_RendersTemplateProperties()
    {
        var writer = new StringWriter();
        var property = new LogEventProperty("User", new ScalarValue("alice"));

        new EmailBodyFormatter().Format(CreateEvent("Hello {User}", property), writer);

        Assert.Equal("Hello &quot;alice&quot;", writer.ToString());
    }

    [Fact]
    public void FormatBatch_WrapsEachEventInTableRow()
    {
        var writer = new StringWriter();

        new EmailBodyFormatter().FormatBatch(new[] { CreateEvent("one"), CreateEvent("two") }, writer);

        Assert.Equal("<table><tr>one</tr><tr>two</tr></table>", writer.ToString());
    }

    [Fact]
    public void FormatBatch_EmptyBatch_WritesEmptyTable()
    {
        var writer = new StringWriter();

        new EmailBodyFormatter().FormatBatch(Array.Empty<LogEvent>(), writer);

        Assert.Equal("<table></table>", writer.ToString());
    }
}
