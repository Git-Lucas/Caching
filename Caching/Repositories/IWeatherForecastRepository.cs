using Caching.Entities;

namespace Caching.Repositories;

public interface IWeatherForecastRepository
{
    Task<WeatherForecast[]> GetWeatherForecastsAsync(int skip, int take);
}
