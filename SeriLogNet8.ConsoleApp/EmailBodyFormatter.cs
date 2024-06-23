using Serilog.Events;
using Serilog.Sinks.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SeriLogNet8.ConsoleApp
{
   
    public class EmailBodyFormatter : IBatchTextFormatter
    {
        public void FormatBatch(IEnumerable<LogEvent> logEvents, TextWriter output)
        {
            output.Write("<table>");
            foreach (var logEvent in logEvents)
            {
                output.Write("<tr>");
                Format(logEvent, output);
                output.Write("</tr>");
            }

            output.Write("</table>");
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            using var buffer = new StringWriter();
            logEvent.RenderMessage(buffer);
            output.Write(WebUtility.HtmlEncode(buffer.ToString()));
        }
    }
}
