using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services;

public class WeatherApiService : IWeatherService
{
    private readonly HttpClient _http;
    // ВСТАВЬ СВОЙ API-КЛЮЧ ПЕРЕД ЗАПУСКОМ
    private const string ApiKey = "YOUR_API_KEY_HERE";
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5";

    public WeatherApiService()
    {
        _http = new HttpClient();
        _http.DefaultRequestHeaders.AcceptLanguage.ParseAdd("ru");
    }

    public async Task<WeatherInfo> GetWeatherAsync(string city)
    {
        var url = $"{BaseUrl}/weather?q={Uri.EscapeDataString(city)}&appid={ApiKey}&units=metric&lang=ru";
        var json = await _http.GetStringAsync(url);
        
        var response = JsonSerializer.Deserialize<OpenWeatherResponse>(json);
        if (response == null) throw new InvalidOperationException("Пустой ответ API");

        var weather = response.Weather.FirstOrDefault();
        var desc = weather?.Description ?? "Нет данных";
        
        return new WeatherInfo
        {
            City = response.City,
            Temperature = Math.Round(response.Main.Temp, 1),
            Condition = char.ToUpper(desc[0]) + desc.Substring(1),
            IconUrl = $"https://openweathermap.org/img/wn/{weather?.Icon ?? "01d"}@2x.png",
            Humidity = response.Main.Humidity,
            WindSpeed = Math.Round(response.Wind.Speed, 1),
            Pressure = (int)(response.Main.Pressure * 0.750062)
        };
    }

    public async Task<List<ForecastItem>> GetForecastAsync(string city)
    {
        var url = $"{BaseUrl}/forecast?q={Uri.EscapeDataString(city)}&appid={ApiKey}&units=metric&lang=ru";
        var json = await _http.GetStringAsync(url);
        
        using var doc = JsonDocument.Parse(json);
        var list = doc.RootElement.GetProperty("list");
        var forecast = new List<ForecastItem>();
        
        foreach (var item in list.EnumerateArray())
        {
            var dateText = item.GetProperty("dt_txt").GetString()!.Split(' ')[0];
            if (forecast.All(f => f.Date != dateText) && forecast.Count < 5)
            {
                forecast.Add(new ForecastItem
                {
                    Date = dateText,
                    TempMin = item.GetProperty("main").GetProperty("temp_min").GetDouble(),
                    TempMax = item.GetProperty("main").GetProperty("temp_max").GetDouble(),
                    Condition = item.GetProperty("weather")[0].GetProperty("description").GetString()!
                });
            }
        }
        return forecast;
    }
}