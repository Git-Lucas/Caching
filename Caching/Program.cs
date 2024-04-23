using Caching.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

var app = builder.Build();

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

string nameEndpoint = "GetWeatherForecast";
app.MapGet("/weatherforecast", ([FromServices]IMemoryCache memoryCache) =>
{
    if (!memoryCache.TryGetValue(nameEndpoint, out WeatherForecast[]? forecast))
    {
        forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();

        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(5));

        memoryCache.Set(nameEndpoint, forecast, cacheEntryOptions);
    }

    return forecast;
})
.WithName(nameEndpoint)
.WithOpenApi();

app.Run();