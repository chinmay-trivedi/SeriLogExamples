using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Context;
using SeriLogNetFrameWork;

namespace SeriLogNetFrameWork
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var configuration = new ConfigurationBuilder()               
            //   .SetBasePath(Directory.GetCurrentDirectory())
            //   .AddJsonFile(path: "appsettings.json",
            //                optional: false,
            //                reloadOnChange: true)
            //   .AddJsonFile(path: $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "production"}.json",
            //                optional: true,
            //                reloadOnChange: true)
            //   .Build();
           

            Log.Logger = new LoggerConfiguration()
               //.ReadFrom.Configuration(configuration)
                .MinimumLevel.Debug()
                .Enrich.WithThreadId()
                .Enrich.FromLogContext()
                .ReadFrom.AppSettings()
                //.WriteTo.Console()
                //.WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Hello, world!");
            Log.Information("No contextual properties");

            using (LogContext.PushProperty("A", 1))
            {
                Log.Information("Carries property A = 1");

                using (LogContext.PushProperty("A", 2))
                using (LogContext.PushProperty("B", 1))
                {
                    Log.Information("Carries A = 2 and B = 1");
                }

                Log.Information("Carries property A = 1, again");
            }

            int a = 10, b = 0;
            try
            {
                Log.Debug("Dividing {A} by {B}", a, b);
                Console.WriteLine(a / b);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong");
                Console.ReadKey();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
