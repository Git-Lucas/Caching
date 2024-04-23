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
app.MapGet("/weatherforecast", async ([FromServices]IMemoryCache memoryCache) =>
{
    WeatherForecast[]? forecast = await memoryCache.GetOrCreateAsync(
        nameEndpoint,
        cacheEntry =>
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3);
            return Task.FromResult(Enumerable
                                   .Range(1, 5)
                                   .Select(index =>
                                   new WeatherForecast
                                   (
                                       DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                       Random.Shared.Next(-20, 55),
                                       summaries[Random.Shared.Next(summaries.Length)]
                                   ))
                                   .ToArray());
        });
    

    return forecast;
})
.WithName(nameEndpoint)
.WithOpenApi();

app.Run();