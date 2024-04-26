using Caching.Entities;

namespace Caching.Repositories;

public class WeatherForecastRepository : IWeatherForecastRepository
{
    private readonly string[] _summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    public async Task<int> CreateAsync(WeatherForecast weatherForecast)
    {
        int weatherForecastId = new Random().Next(101, 1000);
        
        await Task.Delay(1000);

        return weatherForecastId;
    }

    public async Task<WeatherForecast[]> GetPagedAsync(int skip, int take)
    {
        take = ValidateTake(take);

        WeatherForecast[] forecasts = Enumerable.Range(1, take).Select(index =>
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

    private int ValidateTake(int take)
    {
        if (take < 0 || take > 100)
        {
            return 100;
        }

        return take;
    }
}
