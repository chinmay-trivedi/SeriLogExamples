﻿using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using Serilog.Templates;
using Serilog.Templates.Themes;
using Serilog.Context;
using System.Diagnostics;
using System.IO;
using Serilog.Sinks.EmailPickup;

var configFile = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())                
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            //.ReadFrom.Configuration(configFile)
            //.WriteTo.Logger( lev => lev
            //.Filter.ByIncludingOnly( l => l.Level == LogEventLevel.Error)
            //.WriteTo.EventLog("SerilogConsoleApp.Eample", manageEventSource:true))
            .WriteTo.EmailPickup(
                fromEmail: "doNotReply@example.com",
                toEmail: "supportTeam@example.com",
                pickupDirectory: @"c:\logs\emailpickup",
                subject: "There is something wrong with consoleapp",
                fileExtension: ".eml",
                restrictedToMinimumLevel: LogEventLevel.Error)
            .WriteTo.Console(new JsonFormatter())
                .Enrich.WithProperty("Application", "SeriLogExamples")
                .Enrich.WithDemystifiedStackTraces()
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .Enrich.WithThreadName()                
                //.WriteTo.Console(new ExpressionTemplate("{ {@t, @mt, @l: if @l = 'Information' then undefined() else @l, @x, ..@p} }\n", theme: TemplateTheme.Code))
                .WriteTo.Console(new ExpressionTemplate("[{@t:HH:mm:ss} {@l:u3} " + "{Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)}] {@m}\n{@x}", theme: TemplateTheme.Literate))
                 .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .Enrich.WithThreadName()
            //To repart part of the temlate for all the members
            //.WriteTo.Console(new ExpressionTemplate("[{@t:HH:mm:ss} {@l:u3}] {@m}\n" + "{#each name, value in @p}   {name} = {value}\n{#end}{@x}", theme: TemplateTheme.Grayscale))
            //.WriteTo.Debug()
            //.WriteTo.Email(
            //            fromEmail: "no-reply@example.com",
            //            toEmail: "firstname.lastname@example.com",
            //            mailServer: "smtp.example.com",
            //            outputTemplate: "[{Level}] {Message}{NewLine}{Exception}",
            //            mailSubject: "Application Error Logging")
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(l => l.Level == LogEventLevel.Debug)
                .WriteTo.Debug())
             .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .Enrich.WithThreadName()
            .WriteTo.Logger(lc => lc
                .Filter.ByExcluding(l => l.Level == LogEventLevel.Information || l.Level == LogEventLevel.Debug || l.Level == LogEventLevel.Warning || l.Properties["EventType"].Equals("System"))
                .WriteTo.File(Directory.GetParent(AppContext.BaseDirectory).FullName + @"\\Logs\\log-Information-.txt", rollingInterval: RollingInterval.Day))
             .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .Enrich.WithThreadName()
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(l => l.Level == LogEventLevel.Error || l.Level == LogEventLevel.Fatal)
                .WriteTo.File(Directory.GetParent(AppContext.BaseDirectory).FullName + @"\\Logs\\log-Error-.txt", rollingInterval: RollingInterval.Day))
             .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .Enrich.WithThreadName()
            .CreateLogger();

SelfLog.Enable(Console.Out);


try
{
    var sw = new Stopwatch();

    sw.Start();
    Serilog.Log.Logger.Information("*************************** Read DailyFile Started *****************************");
    Serilog.Log.Logger.Debug("*************************** Read DailyFile Started *****************************");

    var appLogger = new LoggerConfiguration()
                    //.ReadFrom.Configuration(configFile)
                    .WriteTo.Console(new CompactJsonFormatter())
                    .Enrich.WithThreadId()
                    .Enrich.WithThreadName()
                    .Enrich.FromLogContext()
                    .CreateLogger();

    appLogger.Information("*** AppLogger Activated ***");

    for (global::System.Int32 i = 0; i < 100; i++)
    {
        Thread thread = new Thread(new ThreadStart(Worker));
        thread.Start();

        //Thread t = new Thread(() => StartupB(port, path));
        //t.Start();

        appLogger.Information("*** AppLogger Value::{0}{1}", @i, Thread.CurrentThread);

        Console.WriteLine(@i);

        if (i==50)
        {
            
            // ATTENTION : Below line is to create an infinite loop. Use CTRL + C to kill the execution and check the log files
            // This is the easier way to demonstrate and how to write bunch of error lines into log file, below line is at all not necessary
            // Below line is an optional line of code you can comment below line and execute the code as well.
           //  Console.WriteLine(i--);

            Serilog.Log.Error("File Watcher Error on {ElapsedTime:mm\\:ss\\.fff} --- {0} {EventType} for file type {1} deactivated...", sw.Elapsed, sw.ElapsedMilliseconds, i, "System");
            appLogger.Error("*** AppLogger ErrorValue::{0}", i);
        }
        
    }
    appLogger.Information("*** AppLogger DeActivated ***");
    Console.ReadKey();

    sw.Stop();
    Serilog.Log.Logger.Information("File Watcher on {0} for file type {1} deactivated...", sw.Elapsed, sw.ElapsedMilliseconds);
    Serilog.Log.Logger.Debug("File Watcher on {0} for file type {1} deactivated...", sw.Elapsed, sw.ElapsedMilliseconds);

    
    Serilog.Log.Logger.Information("*************************** Read DailyFile Completed *****************************");
    Serilog.Log.Logger.Debug("*************************** Read DailyFile Completed *****************************");

}
catch (Exception ex)
{
    Log.Fatal(ex, "File Watcher failed with {err}...", ex.InnerException);
    throw;
}
finally
{
    Log.Information("File Watcher stopped...");    
    Log.CloseAndFlush();

}

static void Worker()
{
    // Code to execute in the new thread
    Log.Information("Created a new ThreadName:{0}",Thread.CurrentThread.Name);
}