using System;

namespace ExperienceHttps
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        private string summary;
        public string Summary {
            get { return summary; }
            set { summary = "88888"; } }
        }
}
