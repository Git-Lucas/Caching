using Caching.Cache;
using Caching.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Caching.Repositories;

public class CachedWeatherForecastRepository(
    [FromKeyedServices(nameof(WeatherForecastRepository))]
    IWeatherForecastRepository decorated,
    IDistributedCache distributedCache) : IWeatherForecastRepository
{
    private readonly IWeatherForecastRepository _decorated = decorated;
    private readonly IDistributedCache _distributedCache = distributedCache;

    public async Task<WeatherForecast[]> GetWeatherForecastsAsync()
    {
        string? forecastsJson = await _distributedCache.GetStringAsync(CacheKeys.GetWeatherForecasts);

        WeatherForecast[] weatherForecasts;
        if (string.IsNullOrEmpty(forecastsJson))
        {
            weatherForecasts = await _decorated.GetWeatherForecastsAsync();

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(3))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(10));

            await _distributedCache.SetStringAsync(CacheKeys.GetWeatherForecasts, 
                                                  JsonSerializer.Serialize(weatherForecasts),
                                                  options);

            return weatherForecasts;
        }

        weatherForecasts = JsonSerializer
                .Deserialize<WeatherForecast[]>(forecastsJson)
                ?? throw new Exception($"Unable to deserialize to array of {nameof(WeatherForecast)}");

        return weatherForecasts;
    }
}
