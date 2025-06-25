namespace Bp_project.Models
{
    public class WeatherHistoryEntry
    {
        public string City { get; set; }
        public string Description { get; set; }
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public DateTime DateTime { get; set; }
    }
} 