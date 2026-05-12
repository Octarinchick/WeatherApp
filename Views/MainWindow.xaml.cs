using System.Windows;
using System.Windows.Input;
using WeatherApp.ViewModels;
using WeatherApp.Services;

namespace WeatherApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(
            new WeatherApiService(),
            new DataStorageService()
        );
    }

    private void HistoryItem_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is System.Windows.Controls.TextBlock tb && 
            DataContext is MainWindowViewModel vm)
        {
            vm.LoadFromHistoryCommand.Execute(tb.Text);
        }
    }
}