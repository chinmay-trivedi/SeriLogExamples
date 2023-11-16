﻿using Serilog.Events;
using Serilog;

namespace SeriLogWebAPI
{
    public static class LogHelper
    {
        public static void EnrichFromRequest(
            IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var request = httpContext.Request;
            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Scheme);
            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            var endpoint = httpContext.GetEndpoint();
            if (endpoint is object)
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }

        public static LogEventLevel CustomGetLevel(HttpContext ctx, double _, Exception ex) =>
            ex != null
                ? LogEventLevel.Error
                : ctx.Response.StatusCode > 499
                    ? LogEventLevel.Error
                    : LogEventLevel.Debug; //Debug instead of Information

        /// <summary>
        /// Create a <see cref="Serilog.AspNetCore.RequestLoggingOptions.GetLevel"> method that
        /// uses the default logging level, except for the specified endpoint names, which are
        /// logged using the provided <paramref name="traceLevel" />.
        /// </summary>
        /// <param name="traceLevel">The level to use for logging "trace" endpoints</param>
        /// <param name="traceEndpointNames">The display name of endpoints to be considered "trace" endpoints</param>
        /// <returns></returns>
        public static Func<HttpContext, double, Exception, LogEventLevel> GetLevel(LogEventLevel traceLevel, params string[] traceEndpointNames)
        {
            if (traceEndpointNames is null || traceEndpointNames.Length == 0)
            {
                throw new ArgumentNullException(nameof(traceEndpointNames));
            }

            return (ctx, _, ex) =>
                IsError(ctx, ex)
                ? LogEventLevel.Error
                : IsTraceEndpoint(ctx, traceEndpointNames)
                    ? traceLevel
                    : LogEventLevel.Information;
        }

        static bool IsError(HttpContext ctx, Exception ex)
            => ex != null || ctx.Response.StatusCode > 499;

        static bool IsTraceEndpoint(HttpContext ctx, string[] traceEndpoints)
        {
            var endpoint = ctx.GetEndpoint();
            if (endpoint is object)
            {
                for (var i = 0; i < traceEndpoints.Length; i++)
                {
                    if (string.Equals(traceEndpoints[i], endpoint.DisplayName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
