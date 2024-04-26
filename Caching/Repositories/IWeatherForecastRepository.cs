using Caching.Entities;

namespace Caching.Repositories;

public interface IWeatherForecastRepository
{
    Task<int> CreateAsync(WeatherForecast weatherForecast);
    Task<WeatherForecast[]> GetPagedAsync(int skip, int take);
}
