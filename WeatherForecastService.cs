namespace graphQLsample
{
    public static class WeatherForecastService
    {
        private static List<WeatherForecast> _forecasts = new();
        private static int _nextId = 1;

        static WeatherForecastService()
        {
            InitializeData();
        }

        private static void InitializeData()
        {
            _forecasts.Add(new WeatherForecast
            {
                Id = _nextId++,
                Owner = "MY_ID",
                Date = DateOnly.FromDateTime(DateTime.Now),
                TemperatureC = 25,
                Summary = "Mild"
            });
        }

        public static IEnumerable<WeatherForecast> GetAllForecasts()
        {
            return _forecasts.AsReadOnly();
        }

        public static WeatherForecast? GetForecastById(int id)
        {
            return _forecasts.FirstOrDefault(f => f.Id == id);
        }

        public static WeatherForecast AddForecast(string owner, DateOnly date, int temperatureC, string? summary)
        {
            var forecast = new WeatherForecast
            {
                Id = _nextId++,
                Owner = owner,
                Date = date,
                TemperatureC = temperatureC,
                Summary = summary
            };

            _forecasts.Add(forecast);
            return forecast;
        }
    }
}
