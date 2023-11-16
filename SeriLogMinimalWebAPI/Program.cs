using Serilog;

var builder = WebApplication.CreateBuilder(args);


//remove default logging providers
builder.Logging.ClearProviders();

//SeriLog Config

var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

//Register Serilog
builder.Logging.AddSerilog(logger);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// How to use logging inside your method
app.MapGet("/", (ILoggerFactory loggerFactory) => {
    var logger = loggerFactory.CreateLogger("index");
    logger.LogInformation("index called");
    return "Hello world";
});

// How to use logging in Program.cs file		
app.Logger.LogInformation("The application started");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("weatherforecast");
    logger.LogInformation("Get WeatherForcast called");
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
