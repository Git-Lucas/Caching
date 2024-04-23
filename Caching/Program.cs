using Caching.Data;
using Caching.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IWeatherForecastData, WeatherForecastData>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

string nameEndpoint = "GetWeatherForecast";
app.MapGet("/api/weatherforecast", async ([FromServices]IMemoryCache memoryCache, IWeatherForecastData weatherForecastData) =>
{
    WeatherForecast[]? forecast = await memoryCache.GetOrCreateAsync(
       nameEndpoint,
       cacheEntry =>
       {
           cacheEntry.SetSlidingExpiration(TimeSpan.FromSeconds(3));
           cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
           return weatherForecastData.GetWeatherForecastsAsync();
       });


    return forecast;
})
.WithName(nameEndpoint)
.WithOpenApi();

app.Run();