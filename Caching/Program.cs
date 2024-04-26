using Caching;
using Caching.Entities;
using Caching.Repositories;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddStackExchangeRedisCache(options =>
{
    string connection = builder.Configuration
        .GetConnectionString("Redis")
        ?? throw new Exception("Unable to read connection string from cache server.");

    options.Configuration = connection;
});

builder.Services.AddDecoratedScoped<IWeatherForecastRepository, 
                                    CachedWeatherForecastRepository, 
                                    WeatherForecastRepository>(nameof(WeatherForecastRepository));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/weatherforecast", 
           async ([FromServices] IWeatherForecastRepository weatherForecastRepository, 
                         [FromQuery] int skip = 0, 
                         [FromQuery] int take = 5) =>
{
    WeatherForecast[] weatherForecasts = await weatherForecastRepository.GetWeatherForecastsAsync(skip, take);
    return weatherForecasts;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();