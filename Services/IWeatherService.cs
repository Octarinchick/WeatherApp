using WeatherApp.Models;

namespace WeatherApp.Services;

public interface IWeatherService
{
    Task<WeatherInfo> GetWeatherAsync(string city);
    Task<List<ForecastItem>> GetForecastAsync(string city); // Опционально
}