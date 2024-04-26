using Caching;
using Caching.DTOs;
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
    WeatherForecast[] weatherForecasts = await weatherForecastRepository.GetPagedAsync(skip, take);

    GetPagedResponse<WeatherForecast> weatherForecastsPaginated = new(100,
                                                                      skip,
                                                                      take,
                                                                      weatherForecasts);

    return weatherForecastsPaginated;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapPost("/api/weatherforecast",
            async ([FromBody] WeatherForecast weatherForecast,
                          [FromServices] IWeatherForecastRepository weatherForecastRepository) =>
{
    int weatherForecastId = await weatherForecastRepository.CreateAsync(weatherForecast);

    return weatherForecastId;
})
.WithName("PostWeatherForecast")
.WithOpenApi();

app.Run();