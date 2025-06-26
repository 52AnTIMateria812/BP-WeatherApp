namespace Bp_project.Models
{
    public class WeatherInfo
    {
        public required string City { get; set; }
        public required string Description { get; set; }
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public required string Icon { get; set; }
        public DateTime DateTime { get; set; }
    }
} 