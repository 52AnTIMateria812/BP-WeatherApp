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
        private WeatherInfo? _weather;
        public WeatherInfo? Weather
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
            if (Weather == null)
                return;
            var entry = new WeatherHistoryEntry
            {
                City = Weather.City ?? string.Empty,
                Description = Weather.Description ?? string.Empty,
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
                    var fileContent = File.ReadAllText(HistoryFile);
                    if (!string.IsNullOrWhiteSpace(fileContent))
                    {
                        var deserialized = JsonSerializer.Deserialize<List<WeatherHistoryEntry>>(fileContent);
                        if (deserialized != null)
                            history = deserialized;
                    }
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
                            City = root.TryGetProperty("name", out var cityProp) ? cityProp.GetString() ?? string.Empty : string.Empty,
                            Description = root.TryGetProperty("weather", out var weatherArr) && weatherArr.GetArrayLength() > 0 && weatherArr[0].TryGetProperty("description", out var descProp) ? descProp.GetString() ?? string.Empty : string.Empty,
                            Temperature = root.GetProperty("main").GetProperty("temp").GetDouble(),
                            FeelsLike = root.GetProperty("main").GetProperty("feels_like").GetDouble(),
                            Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32(),
                            WindSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
                            Icon = root.TryGetProperty("weather", out var weatherArr2) && weatherArr2.GetArrayLength() > 0 && weatherArr2[0].TryGetProperty("icon", out var iconProp) ? $"https://openweathermap.org/img/wn/{iconProp.GetString()}@2x.png" : string.Empty,
                            DateTime = DateTime.Now
                        };
                        SaveHistory();
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name ?? string.Empty));
        }
    }
} 