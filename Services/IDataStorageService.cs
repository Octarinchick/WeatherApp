using System.Collections.ObjectModel;

namespace WeatherApp.Services;

public interface IDataStorageService
{
    void SaveHistory(ObservableCollection<string> history);
    ObservableCollection<string> LoadHistory();
    void SaveSettings(string lastCity, bool isDarkTheme);
    (string LastCity, bool IsDarkTheme) LoadSettings();
}