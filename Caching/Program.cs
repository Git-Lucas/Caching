using Caching.Cache;
using Caching.Data;
using Caching.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnectionString");
    options.InstanceName = "MyInstance";
});
builder.Services.AddScoped<IWeatherForecastData, WeatherForecastData>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/weatherforecast", async ([FromServices] IDistributedCache distributedCache, IWeatherForecastData weatherForecastData) =>
{
    byte[]? forecastBytes = await distributedCache.GetAsync(CacheKeys.GetWeatherForecast);

    if (forecastBytes != null)
    {
        string forecastJson = Encoding.UTF8.GetString(forecastBytes);
        return forecastJson;
    }

    WeatherForecast[] forecastsFromDatabase = await weatherForecastData.GetWeatherForecastsAsync();
    string jsonForecastsFromDatabase = JsonSerializer.Serialize(forecastsFromDatabase, 
                                                                new JsonSerializerOptions() { WriteIndented = true });
    byte[] encodedforecastsFromDatabase = Encoding.UTF8.GetBytes(jsonForecastsFromDatabase);

    DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromSeconds(3))
        .SetAbsoluteExpiration(TimeSpan.FromSeconds(10));

    await distributedCache.SetAsync(CacheKeys.GetWeatherForecast, encodedforecastsFromDatabase, options);

    return jsonForecastsFromDatabase;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();