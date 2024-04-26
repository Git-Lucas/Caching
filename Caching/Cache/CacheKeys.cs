namespace Caching.Cache;

public static class CacheKeys
{
    public const string GetWeatherForecastsPrefix = "WeatherForecasts";
    public static string GetWeatherForecasts(int skip, int take)
    {
        return $"{GetWeatherForecastsPrefix}_{skip}-{take}";
    }
}
