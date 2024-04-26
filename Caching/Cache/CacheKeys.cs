namespace Caching.Cache;

public static class CacheKeys
{
    public static string GetWeatherForecasts(int skip, int take)
    {
        return $"WeatherForecasts_{skip}-{take}";
    }
}
