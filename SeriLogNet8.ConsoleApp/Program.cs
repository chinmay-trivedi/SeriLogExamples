// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Templates.Themes;
using Serilog.Templates;
using Serilog.Events;
using Serilog.Formatting.Compact;
using SeriLogNet8.ConsoleApp;
using Microsoft.Extensions.Hosting.Internal;

var configFile = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())                
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(configHost =>
    {
        configHost.SetBasePath(Directory.GetCurrentDirectory());
        configHost.AddJsonFile("appsettings.json", optional: true);
    })
    //.ConfigureServices(services =>
    //{
    //})
    //.ConfigureAppConfiguration((hostingContext, config) =>
    //{
    //    config.Sources.Clear();
    //    config.AddConfiguration(configFile);
    //})
    //.ConfigureLogging(logging =>
    //{
    //    logging.ClearProviders();
    //    logging.AddConsole();
    //})
    .UseSerilog((context, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration)
        //configuration.WriteTo.Console(new ExpressionTemplate("[{@t:HH:mm:ss} {@l:u3} " + "{Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)}] {@m}\n{@x}", theme: TemplateTheme.Literate))
        //.Enrich.FromLogContext().Filter.ByIncludingOnly(l => l.Level >= LogEventLevel.Information)
        .Enrich.WithProperty("Application", "MyConsoleApp")
        .Enrich.FromLogContext()
        .WriteTo.Map("SourceContext", null, (sc, wt) => wt.File(new CompactJsonFormatter(), sc == "Program" ? @"Logs//specialLog.log" :
        @"C:\\Program Files (x86)\\Temp\\log\\AppLog.log"
        ))
        //.Filter.ByIncludingOnly(l => l.Properties["SourceContext"].Equals("Program"))         
        //.Enrich.FromLogContext().Filter.ByIncludingOnly(l => l.Properties["SourceContext"].Equals("Program"))         
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            //.Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadName();
        //.WriteTo.Email(
        //    options: new()
        //    {          
        //        From = "no-reply@example.com",
        //        To = { "supportteam@example.com" },
        //        Host =  "webmail.example.com",
        //        Body = new EmailBodyFormatter(),
        //        IsBodyHtml = true,
        //        //Subject = "Application Error Logging"
        //    },
        //    batchingOptions: new()
        //    {
        //        BatchSizeLimit = 10,
        //        Period = TimeSpan.FromSeconds(30)
        //    });
            //if (context.HostingEnvironment.IsDevelopment())
            //{
            //    configuration
            //        .WriteTo.Console()
            //        .WriteTo.Debug();
            //}
            //else 
            //{
            //    configuration
            //    //.MinimumLevel.Equals(LogLevel.Debug)
            //    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            //    .WriteTo.Console(LogEventLevel.Error);
            //}
    })
    .Build();


//Log.Logger = new LoggerConfiguration()
//            //.ReadFrom.Configuration(configFile)
//            .WriteTo.Console(new ExpressionTemplate("[{@t:HH:mm:ss} {@l:u3} " + "{Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)}] {@m}\n{@x}", theme: TemplateTheme.Literate))                             
//            .Enrich.WithThreadId()
//            .Enrich.WithProcessId()
//            .Enrich.WithThreadName()
//            .CreateLogger();

SelfLog.Enable(Console.Out);
try
{

    // logging
    var logger = host.Services.GetService<ILogger<Program>>();

    logger.LogDebug("Test serilog");
    logger.LogError("Error serilog");
    logger.LogCritical("Critical serilog");
    logger.LogInformation("Info serilog");

    IMyService myService = new MyService(configFile, logger);

    myService.DoSomething();

    Console.WriteLine("Hello, World!");
    Thread thread = new Thread(new ThreadStart(Worker));
    thread.Start();
    Console.ReadLine();
    
}
catch (Exception ex)
{
    Log.Logger.Fatal(ex, "File Watcher failed with {err}...", ex.InnerException);
    throw;
}
finally
{
    Log.Logger.Information("File Watcher stopped...");
    Log.CloseAndFlush();

}
static void Worker()
{
    //Log.Logger.ForContext();

  

    // Code to execute in the new thread
    Log.Logger.Fatal("Created a new ThreadName:{0}", Thread.CurrentThread.Name);
}
