using Caching.Entities;

namespace Caching.Repositories;

public class WeatherForecastRepository : IWeatherForecastRepository
{
    private readonly string[] _summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    public async Task<WeatherForecast[]> GetWeatherForecastsAsync()
    {
        WeatherForecast[] forecasts = Enumerable.Range(1, 20).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                _summaries[Random.Shared.Next(_summaries.Length)]
            ))
            .ToArray();

        await Task.Delay(2000);

        return forecasts;
    }
}
