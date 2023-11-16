using Serilog;
using Serilog.Events;
using SeriLogWebAPI;
using SeriLogWebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);


var config = new ConfigurationBuilder()
              //.AddJsonFile("appsettings.json")
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
              .AddEnvironmentVariables()
              .Build();

//This is required to enable and forward everything to Serilog Config sinks
builder.Host.UseSerilog((ctx, lc) => lc
    //.WriteTo.Console()
    .Enrich.FromLogContext()
    .Enrich.WithThreadId().Enrich.WithProcessId()
    .Enrich.WithProperty("Env", "QAS")
    .Enrich.WithProperty("Application", "SeriLogWebAPI")
    .ReadFrom.Configuration(ctx.Configuration));

Log.Information("Hello, {Name}!", "world");

Serilog.
Log.ForContext("Env", "QAS")
.Information("Hello {Name} from thread {ThreadId} {AppName}", Environment.GetEnvironmentVariable("USERNAME"), Thread.CurrentThread.ManagedThreadId, Environment.SystemDirectory);

Serilog.Log.ForContext("Env", "QAS").Information("Hello from Progmram.cs file, {Name} {CurrDir}!", Environment.UserName, Environment.CurrentDirectory);
Serilog.Log.ForContext("Env", "QAS").Information("Error logging from Progmram.cs file, {Name} {CurrDir}!", Environment.UserName, Environment.CurrentDirectory);




// Add services to the container.

builder.Services.AddControllers(opts => 
{
    opts.Filters.Add<SeriLogPageFilter>();
    opts.Filters.Add<SeriLogActionFilter>();
});

builder.Services.AddHealthChecks();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseSerilogRequestLogging( opts => 
//{
//    opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
//    opts.GetLevel = LogHelper.GetLevel(Serilog.Events.LogEventLevel.Verbose, "Health Checks");
//}); //For SeriLog request logging

app.UseSerilogRequestLogging(options =>
{
    // Customize the message template
    options.MessageTemplate = "Handled {RequestPath}";

    // Emit debug-level events instead of the defaults
    options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Error;

    // Attach additional properties to the request completion event
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("RequestUser", httpContext.User.Identity.Name);
        diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress.ToString());
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
        //diagnosticContext.Set("Resource", httpContext.GetMetricsCurrentResourceName());
    };
});
app.UseMiddleware<SerilogRequestLogger>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
