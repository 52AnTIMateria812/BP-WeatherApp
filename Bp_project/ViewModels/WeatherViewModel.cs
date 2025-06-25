using System.ComponentModel;
using System.Runtime.CompilerServices;
using Bp_project.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;

namespace Bp_project.ViewModels
{
    public class WeatherViewModel : INotifyPropertyChanged
    {
        private WeatherInfo _weather;
        public WeatherInfo Weather
        {
            get => _weather;
            set
            {
                _weather = value;
                OnPropertyChanged();
            }
        }

        private AppSettings _settings;
        private const string HistoryFile = "weather_history.json";
        public WeatherViewModel()
        {
            _settings = SettingsWindow.LoadSettings();
        }
        public void UpdateSettings(AppSettings settings)
        {
            _settings = settings;
        }
        private void SaveHistory()
        {
            var entry = new WeatherHistoryEntry
            {
                City = Weather.City,
                Description = Weather.Description,
                Temperature = Weather.Temperature,
                Humidity = Weather.Humidity,
                WindSpeed = Weather.WindSpeed,
                DateTime = Weather.DateTime
            };
            List<WeatherHistoryEntry> history = new();
            if (File.Exists(HistoryFile))
            {
                try
                {
                    history = JsonSerializer.Deserialize<List<WeatherHistoryEntry>>(File.ReadAllText(HistoryFile));
                }
                catch { }
            }
            history.Add(entry);
            File.WriteAllText(HistoryFile, JsonSerializer.Serialize(history));
        }
        public async Task LoadWeatherAsync()
        {
            using (var client = new HttpClient())
            {
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={_settings.City}&appid={_settings.ApiKey}&units=metric&lang=ru";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        var root = doc.RootElement;
                        Weather = new WeatherInfo
                        {
                            City = root.GetProperty("name").GetString(),
                            Description = root.GetProperty("weather")[0].GetProperty("description").GetString(),
                            Temperature = root.GetProperty("main").GetProperty("temp").GetDouble(),
                            FeelsLike = root.GetProperty("main").GetProperty("feels_like").GetDouble(),
                            Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32(),
                            WindSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
                            Icon = $"https://openweathermap.org/img/wn/{root.GetProperty("weather")[0].GetProperty("icon").GetString()}@2x.png",
                            DateTime = DateTime.Now
                        };
                        SaveHistory();
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
} 