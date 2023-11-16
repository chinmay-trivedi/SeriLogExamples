using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using SeriLogWithEFFilter.Data.Context;
using SeriLogWithEFFilter.Data.Models;
using System.Collections;
using System.Diagnostics;

var configFile = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())                
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .ReadFrom.Configuration(configFile)
            .WriteTo.Async(db => db.File("Logs/My.Database..Logs..log"), bufferSize: 1024)
                    .Filter.ByIncludingOnly(l => l.Level == LogEventLevel.Error || l.Level == LogEventLevel.Fatal)
            //.WriteTo.Console()
            //.WriteTo.Logger(lc => lc
            //    .Filter.ByExcluding(l => l.Level == LogEventLevel.Information || l.Level == LogEventLevel.Debug || l.Level == LogEventLevel.Warning )
            //    .WriteTo.File(Directory.GetParent(AppContext.BaseDirectory).FullName + @"\\Logs\\log-Information-.txt", rollingInterval: RollingInterval.Day))
            //.WriteTo.Logger(lc => lc
            //    .Filter.ByIncludingOnly(l => l.Level == LogEventLevel.Error || l.Level == LogEventLevel.Fatal)
            //    .WriteTo.File(Directory.GetParent(AppContext.BaseDirectory).FullName + @"\\Logs\\log-Error-.txt", rollingInterval: RollingInterval.Day))
            .CreateLogger();

SelfLog.Enable(Console.Out);


try
{
    var sw = new Stopwatch();

    sw.Start();
    Serilog.Log.Logger.Information("*************************** Read DailyFile Started *****************************");
    Serilog.Log.Logger.Debug("*************************** Read DailyFile Started *****************************");
    CreateDummyData();


    //for (global::System.Int32 i = 0; i < 100; i++)
    //{
    //    Console.WriteLine(i);

    //    if (i == 50)
    //    {
    //        // ATTENTION : Below line is to create an infinite loop. Use CTRL + C to kill the execution and check the log files
    //        // This is the easier way to demonstrate and how to write bunch of error lines into log file, below line is at all not necessary
    //        // Below line is an optional line of code you can comment below line and execute the code as well.
    //        //Console.WriteLine(i--);


    //        Serilog.Log.Error("File Watcher Error on {2} --- {0} {EventType} for file type {1} deactivated...", sw.Elapsed, sw.ElapsedMilliseconds, i, "System");
    //    }

    //}

    IEnumerable<Blog> blogs = await GetBlogsAsync();

    foreach (var blog in blogs)
    {
        Serilog.Log.Logger.Information("Blog Data: {0} {1}",blog.BlogId, blog.Url);
    }

    Console.ReadKey();

    sw.Stop();
    Serilog.Log.Logger.Information("File Watcher on {0} for file type {1} deactivated...", sw.Elapsed, sw.ElapsedMilliseconds);

    Serilog.Log.Logger.Information("*************************** Read DailyFile Completed *****************************");
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

static void CreateDummyData()
{

    //SqliteConnection sqlite_conn;
    // Create a new database connection:
    //sqlite_conn = new SqliteConnection("Data Source=.\\Data\\MyData.db;Version=3;New=True;Compress=True;");
    // Open the connection:
    try
    {
        //sqlite_conn.Open();        


        using (var db = new MyDBContext())
        {


            // Note: This sample requires the database to be created before running.
            Serilog.Log.Logger.Information("Database path: {db.DbPath}.");
            Serilog.Log.Logger.Information("DB Logging Started......");
            Serilog.Log.Logger.Information("Connection worked!");

            // Create
            Serilog.Log.Logger.Information("Inserting a new blog");
            db.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
            db.SaveChanges();

            // Read
            Serilog.Log.Logger.Information("Querying for a blog");
            var blog = db.Blogs
                .OrderBy(b => b.BlogId)
                .First();

            // Update
            Serilog.Log.Logger.Information("Updating the blog and adding a post");
            blog.Url = "https://devblogs.microsoft.com/dotnet";
            blog.Posts.Add(
                new Post { Title = "Hello World", Content = "I wrote an app using EF Core!" });
            db.SaveChanges();

            // Delete
            //Serilog.Log.Logger.Information("Delete the blog");
            //db.Remove(blog);
            //db.SaveChanges();
        }
        //sqlite_conn.Close();
    }
    catch (Exception ex)
    {
        Serilog.Log.Logger.Error("DB Connection Error......{msg}{innerex}",ex.Message, ex.InnerException);
        Serilog.Log.Logger.Information("Connection didn't work!");
        
    }
   // return sqlite_conn;
}

async Task<IEnumerable<Blog>> GetBlogsAsync()
{
    using (var dbContext = new MyDBContext())
    {
        var result = await dbContext.Blogs.ToListAsync();

        return result;
    }
    
}
   