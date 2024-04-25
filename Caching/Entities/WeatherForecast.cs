namespace Caching.Entities;

public class WeatherForecast
{
    public DateOnly Date { get; private set; }
    public int TemperatureC { get; private set; }
    public string? Summary { get; private set; }
    public int TemperatureF { get; private set; }

    public WeatherForecast(DateOnly date, int temperatureC, string? summary)
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary;
        TemperatureF = 32 + (int)(TemperatureC / 0.5556);
    }
}