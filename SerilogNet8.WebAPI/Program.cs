using Serilog;
using Serilog.Events;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


var config = new ConfigurationBuilder()
              //.AddJsonFile("appsettings.json")
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
              .AddEnvironmentVariables()
              .Build();

//This is required to enable and forward everything to Serilog Config sinks
builder.Host.UseSerilog((ctx, lc) => lc
    //.WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithThreadId().Enrich.WithProcessId()
    .Enrich.WithClientIp()
    .Enrich.WithProperty("Env", "QAS")
    .Enrich.WithProperty("Application", "SeriLogNet8.WebAPI")   
    //.Filter.ByIncludingOnly("SourceContext = 'Microsoft.AspNetCore.Hosting.Diagnostics'")
    );

Log.Information("Hello, {Name}!", "world");

Serilog.
Log.ForContext("Env", "QAS")
.Information("Hello {Name} from thread {ThreadId} {AppName}", Environment.GetEnvironmentVariable("USERNAME"), Thread.CurrentThread.ManagedThreadId, Environment.SystemDirectory);

Serilog.Log.ForContext("Env", "QAS").Information("Hello from Progmram.cs file, {Name} {CurrDir}!", Environment.UserName, Environment.CurrentDirectory);
Serilog.Log.ForContext("Env", "QAS").Information("Error logging from Progmram.cs file, {Name} {CurrDir}!", Environment.UserName, Environment.CurrentDirectory);





builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseSerilogRequestLogging();
//app.UseSerilogRequestLogging(options =>
//{
//    // Customize the message template
//    options.MessageTemplate = "{RemoteIpAddress} {RequestScheme} {RequestHost} {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

//    // Emit debug-level events instead of the defaults
//    options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;

//    // Attach additional properties to the request completion event
//    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
//    {
//        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
//        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
//        diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
//    };
//});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
