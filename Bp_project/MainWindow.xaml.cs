using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Bp_project.ViewModels;

namespace Bp_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WeatherViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new WeatherViewModel();
            DataContext = _viewModel;
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadWeatherAsync();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadWeatherAsync();
        }

        private async void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(SettingsWindow.LoadSettings()) { Owner = this };
            if (settingsWindow.ShowDialog() == true)
            {
                _viewModel.UpdateSettings(settingsWindow.Settings);
                await _viewModel.LoadWeatherAsync();
            }
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new HistoryWindow() { Owner = this };
            historyWindow.ShowDialog();
        }
    }
}