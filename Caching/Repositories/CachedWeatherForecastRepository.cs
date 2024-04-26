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

    public async Task<WeatherForecast[]> GetWeatherForecastsAsync(int skip, int take)
    {
        string? forecastsJson = await _distributedCache.GetStringAsync(CacheKeys.GetWeatherForecasts(skip, take));

        WeatherForecast[] weatherForecasts;
        if (string.IsNullOrEmpty(forecastsJson))
        {
            weatherForecasts = await _decorated.GetWeatherForecastsAsync(skip, take);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(1));

            await _distributedCache.SetStringAsync(CacheKeys.GetWeatherForecasts(skip, take), 
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
