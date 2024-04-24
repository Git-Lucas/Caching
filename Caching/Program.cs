using Caching.Cache;
using Caching.Data;
using Caching.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<MyMemoryCache>();
builder.Services.AddScoped<IWeatherForecastData, WeatherForecastData>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/weatherforecast", async ([FromServices]MyMemoryCache memoryCache, IWeatherForecastData weatherForecastData) =>
{
    WeatherForecast[]? forecast = await memoryCache.Cache.GetOrCreateAsync(
       CacheKeys.GetWeatherForecast,
       cacheEntry =>
       {
           cacheEntry.SetSlidingExpiration(TimeSpan.FromSeconds(3));
           cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
           cacheEntry.SetSize(1024);
           return weatherForecastData.GetWeatherForecastsAsync();
       });

    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();