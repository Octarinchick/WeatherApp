using System.Text.Json.Serialization;

namespace WeatherApp.Models;

// Внутренние записи для десериализации ответа OpenWeatherMap
public record OpenWeatherResponse(
    [property: JsonPropertyName("name")] string City,
    [property: JsonPropertyName("main")] MainData Main,
    [property: JsonPropertyName("weather")] WeatherData[] Weather,
    [property: JsonPropertyName("wind")] WindData Wind
);

public record MainData(
    [property: JsonPropertyName("temp")] double Temp,
    [property: JsonPropertyName("humidity")] int Humidity,
    [property: JsonPropertyName("pressure")] int Pressure
);

public record WeatherData(
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("icon")] string Icon
);

public record WindData(
    [property: JsonPropertyName("speed")] double Speed
);

// Публичная модель для привязки к UI
public class WeatherInfo
{
    public string City { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public int Pressure { get; set; }
}