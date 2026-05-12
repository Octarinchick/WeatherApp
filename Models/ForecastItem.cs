namespace WeatherApp.Models;

public class ForecastItem
{
    public string Date { get; set; } = string.Empty;
    public double TempMin { get; set; }
    public double TempMax { get; set; }
    public string Condition { get; set; } = string.Empty;
}