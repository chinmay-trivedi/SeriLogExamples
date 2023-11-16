using Serilog.Context;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;

namespace SeriLogWebAPI.Middleware
{
  
    public class SerilogRequestLogger
    {
        readonly RequestDelegate _next;

        public SerilogRequestLogger(RequestDelegate next)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            // Push the user name into the log context so that it is included in all log entries
            LogContext.PushProperty("UserName", httpContext.User.Identity.Name);

            // Getting the request body is a little tricky because it's a stream
            // So, we need to read the stream and then rewind it back to the beginning
            string requestBody = "";
            httpContext.Request.EnableBuffering();
            Stream body = httpContext.Request.Body;
            byte[] buffer = new byte[Convert.ToInt32(httpContext.Request.ContentLength)];
            await httpContext.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            requestBody = Encoding.UTF8.GetString(buffer);
            body.Seek(0, SeekOrigin.Begin);
            httpContext.Request.Body = body;

            Log.ForContext("RequestHeaders", httpContext.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
               .ForContext("RequestBody", requestBody)
               .Information("Request information {RequestMethod} {RequestPath} {RequestUser} information", httpContext.Request.Method, httpContext.Request.Path, httpContext.User.Identity.Name);


            // The reponse body is also a stream so we need to:
            // - hold a reference to the original response body stream
            // - re-point the response body to a new memory stream
            // - read the response body after the request is handled into our memory stream
            // - copy the response in the memory stream out to the original response stream
            using (var responseBodyMemoryStream = new MemoryStream())
            {
                var originalResponseBodyReference = httpContext.Response.Body;
                httpContext.Response.Body = responseBodyMemoryStream;

                // await _next(httpContext);

                try
                {
                    await _next(httpContext);
                }
                catch (Exception exception)
                {
                    Guid errorId = Guid.NewGuid();
                    Log.ForContext("Type", "Error")
                        .ForContext("Exception", exception, destructureObjects: true)
                        .Error(exception, exception.Message + ". {@errorId}", errorId);

                    
                    //var result = JsonConvert.SerializeObject(new { error = "Sorry, an unexpected error has occurred", errorId = errorId });
                    var result = JsonSerializer.Serialize(new { error = "Sorry, an unexpected error has occurred", errorId = errorId });

                    
                    
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.StatusCode = 500;
                    await httpContext.Response.WriteAsync(result);
                }

                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                var clientIPAddress1 = GetRemoteHostIpAddressUsingRemoteIpAddress(httpContext);
                var clientIPAddress2 = GetRemoteHostIpAddressUsingXForwardedFor(httpContext);
                var clientIPAddress3 = GetRemoteHostIpAddressUsingXRealIp(httpContext);
                Log.ForContext("RequestBody", requestBody)
                   .ForContext("ResponseBody",responseBody)
                   .ForContext("ClientIP1", clientIPAddress1)
                   .ForContext("ClientIP2", clientIPAddress2)
                   .ForContext("ClientIP3", clientIPAddress3)
                   .Information("Response information {ClientIP1} {ClientIP2} {ClientIP3} {RequestMethod} {RequestPath} {RequestUser} {statusCode}", clientIPAddress1, clientIPAddress2,clientIPAddress3, httpContext.Request.Method, httpContext.Request.Path, httpContext.User.Identity.Name, httpContext.Response.StatusCode);

                await responseBodyMemoryStream.CopyToAsync(originalResponseBodyReference);
            }
        }

        public IPAddress? GetRemoteHostIpAddressUsingRemoteIpAddress(HttpContext httpContext)
        {
             return httpContext.Connection.RemoteIpAddress;

        }

        public IPAddress? GetRemoteHostIpAddressUsingXForwardedFor(HttpContext httpContext)
        {
            IPAddress? remoteIpAddress = null;
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(s => s.Trim());
                foreach (var ip in ips)
                {
                    if (IPAddress.TryParse(ip, out var address) &&
                        (address.AddressFamily is AddressFamily.InterNetwork
                         or AddressFamily.InterNetworkV6))
                    {
                        remoteIpAddress = address;
                        break;
                    }
                }
            }
            return remoteIpAddress;
        }

        public IPAddress? GetRemoteHostIpAddressUsingXRealIp(HttpContext httpContext)
        {
            IPAddress? remoteIpAddress = null;
            var xRealIpExists = httpContext.Request.Headers.TryGetValue("X-Real-IP", out var xRealIp);
            if (xRealIpExists)
            {
                if (!IPAddress.TryParse(xRealIp, out IPAddress? address))
                {
                    return remoteIpAddress;
                }
                var isValidIP = (address.AddressFamily is AddressFamily.InterNetwork
                                 or AddressFamily.InterNetworkV6);

                if (isValidIP)
                {
                    remoteIpAddress = address;
                }
                return remoteIpAddress;
            }
            return remoteIpAddress;
        }
        
    }
}
