using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace WeatherApp.Services;

public class DataStorageService : IDataStorageService
{
    private readonly string _filePath;

    public DataStorageService()
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(folder, "WeatherApp");
        Directory.CreateDirectory(appFolder);
        _filePath = Path.Combine(appFolder, "settings.json");
    }

    private AppSettings Read()
    {
        return File.Exists(_filePath) 
            ? JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(_filePath)) ?? new AppSettings()
            : new AppSettings();
    }

    private void Write(AppSettings settings) => File.WriteAllText(_filePath, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));

    public void SaveHistory(ObservableCollection<string> history)
    {
        var s = Read();
        s.History = new List<string>(history);
        Write(s);
    }

    public ObservableCollection<string> LoadHistory()
    {
        var s = Read();
        return new ObservableCollection<string>(s.History ?? new List<string>());
    }

    public void SaveSettings(string lastCity, bool isDarkTheme)
    {
        var s = Read();
        s.LastCity = lastCity;
        s.IsDarkTheme = isDarkTheme;
        Write(s);
    }

    public (string LastCity, bool IsDarkTheme) LoadSettings()
    {
        var s = Read();
        return (s.LastCity ?? string.Empty, s.IsDarkTheme);
    }

    private class AppSettings
    {
        public string LastCity { get; set; } = string.Empty;
        public bool IsDarkTheme { get; set; }
        public List<string>? History { get; set; } = new();
    }
}