using Caching.Cache;
using Caching.Entities;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace Caching.Repositories;

public class CachedWeatherForecastRepository(
    [FromKeyedServices(nameof(WeatherForecastRepository))]
    IWeatherForecastRepository decorated,
    IDistributedCache distributedCache,
    IConfiguration configuration) : IWeatherForecastRepository
{
    private readonly IWeatherForecastRepository _decorated = decorated;
    private readonly IDistributedCache _distributedCache = distributedCache;
    private readonly ConnectionMultiplexer _redisConnection = ConnectionMultiplexer
        .Connect(configuration.GetConnectionString("Redis")
                 ?? throw new Exception("Unable to read connection string from cache server."));

    public async Task<int> CreateAsync(WeatherForecast weatherForecast)
    {
        int weatherForecastId = await _decorated.CreateAsync(weatherForecast);

        IServer server = _redisConnection.GetServer(_redisConnection.GetEndPoints().First());
        foreach (RedisKey key in server.Keys(pattern: CacheKeys.GetWeatherForecastsPrefix + "*"))
        {
            await _redisConnection.GetDatabase().KeyDeleteAsync(key);
        }

        return weatherForecastId;
    }

    public async Task<WeatherForecast[]> GetPagedAsync(int skip, int take)
    {
        string? forecastsJson = await _distributedCache.GetStringAsync(CacheKeys.GetWeatherForecasts(skip, take));

        WeatherForecast[] weatherForecasts;
        if (string.IsNullOrEmpty(forecastsJson))
        {
            weatherForecasts = await _decorated.GetPagedAsync(skip, take);

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
