using System.IO;
using System.Text.Json;
using System.Windows;
using Bp_project.Models;

namespace Bp_project
{
    public partial class SettingsWindow : Window
    {
        private const string SettingsFile = "settings.json";
        public AppSettings Settings { get; private set; }
        public SettingsWindow(AppSettings currentSettings)
        {
            InitializeComponent();
            Settings = currentSettings;
            ApiKeyBox.Text = Settings.ApiKey;
            CityBox.Text = Settings.City;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.ApiKey = ApiKeyBox.Text.Trim();
            Settings.City = CityBox.Text.Trim();
            File.WriteAllText(SettingsFile, JsonSerializer.Serialize(Settings));
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public static AppSettings LoadSettings()
        {
            if (File.Exists(SettingsFile))
            {
                try
                {
                    return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsFile));
                }
                catch { }
            }
            return new AppSettings { ApiKey = "05f78d0167b51d4907215198462d2357", City = "Saint Petersburg" };
        }
    }
} 