using Caching.Entities;

namespace Caching.Data;

public interface IWeatherForecastData
{
    Task<WeatherForecast[]> GetWeatherForecastsAsync();
}
