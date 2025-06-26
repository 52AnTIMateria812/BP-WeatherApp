using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using Bp_project.Models;

namespace Bp_project
{
    public partial class HistoryWindow : Window
    {
        public HistoryWindow()
        {
            InitializeComponent();
            LoadHistory();
        }
        private void LoadHistory()
        {
            if (File.Exists("weather_history.json"))
            {
                try
                {
                    var fileContent = File.ReadAllText("weather_history.json");
                    if (!string.IsNullOrWhiteSpace(fileContent))
                    {
                        var history = JsonSerializer.Deserialize<List<WeatherHistoryEntry>>(fileContent);
                        if (history != null)
                            HistoryGrid.ItemsSource = history;
                    }
                }
                catch { }
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
} 