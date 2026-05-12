using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IWeatherService _weatherService;
    private readonly IDataStorageService _storageService;

    [ObservableProperty] private string _cityName = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsWeatherVisible))]
    private WeatherInfo? _currentWeather;
    
    [ObservableProperty] private ObservableCollection<ForecastItem> _forecast = new();
    [ObservableProperty] private ObservableCollection<string> _history = new();
    [ObservableProperty] private bool _isDarkTheme;

    // Вычисляемое свойство для управления видимостью карточки погоды
    public bool IsWeatherVisible => CurrentWeather != null;

    public MainWindowViewModel(IWeatherService weatherService, IDataStorageService storageService)
    {
        _weatherService = weatherService;
        _storageService = storageService;

        var settings = _storageService.LoadSettings();
        IsDarkTheme = settings.IsDarkTheme;
        History = _storageService.LoadHistory();
        
        if (!string.IsNullOrWhiteSpace(settings.LastCity))
        {
            CityName = settings.LastCity;
            _ = SearchWeatherAsync();
        }
    }

    [RelayCommand]
    private async Task SearchWeatherAsync()
    {
        if (string.IsNullOrWhiteSpace(CityName.Trim())) 
        {
            ErrorMessage = "Введите название города";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        CurrentWeather = null;
        Forecast.Clear();

        try
        {
            Debug.WriteLine($"🔍 Запрос: {CityName.Trim()}");
            
            CurrentWeather = await _weatherService.GetWeatherAsync(CityName.Trim());
            Debug.WriteLine($"✅ Текущая: {CurrentWeather?.City} {CurrentWeather?.Temperature}°C");
            
            var forecastData = await _weatherService.GetForecastAsync(CityName.Trim());
            Forecast = new ObservableCollection<ForecastItem>(forecastData);
            Debug.WriteLine($"📅 Прогноз: {Forecast.Count} записей");
            
            if (History.Contains(CityName.Trim())) History.Remove(CityName.Trim());
            History.Insert(0, CityName.Trim());
            if (History.Count > 10) History.RemoveAt(10);
            
            _storageService.SaveHistory(History);
            _storageService.SaveSettings(CityName.Trim(), IsDarkTheme);
        }
        catch (HttpRequestException httpEx)
        {
            ErrorMessage = $"Ошибка сети: {httpEx.Message}";
            Debug.WriteLine($"❌ HTTP: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка: {ex.Message}";
            Debug.WriteLine($"❌ Exception: {ex.GetType().Name} | {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            Debug.WriteLine($"🏁 CurrentWeather is null? {CurrentWeather == null}");
        }
    }

    [RelayCommand]
    private void LoadFromHistory(string city)
    {
        CityName = city;
        _ = SearchWeatherAsync();
    }

    partial void OnIsDarkThemeChanged(bool value)
    {
        _storageService.SaveSettings(CityName, value);
        ApplyTheme(value);
    }

    private static void ApplyTheme(bool isDark)
    {
        var app = System.Windows.Application.Current;
        if (app == null) return;
        
        var dict = new System.Windows.ResourceDictionary();
        if (isDark)
        {
            dict["BgBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(30, 30, 30));
            dict["FgBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(230, 230, 230));
            dict["CardBg"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(50, 50, 50));
        }
        else
        {
            dict["BgBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(245, 245, 245));
            dict["FgBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(30, 30, 30));
            dict["CardBg"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
        }

        var toRemove = app.Resources.MergedDictionaries
            .Where(d => d.Contains("BgBrush") || d.Contains("FgBrush") || d.Contains("CardBg"))
            .ToList();
        foreach (var d in toRemove) app.Resources.MergedDictionaries.Remove(d);
        app.Resources.MergedDictionaries.Add(dict);
    }
}