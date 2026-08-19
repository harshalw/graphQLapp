namespace graphQLsample.GraphQL
{
    public class Query
    {
        public IEnumerable<WeatherForecast> GetWeatherForecasts()
        {
            return WeatherForecastService.GetAllForecasts();
        }

        public WeatherForecast? GetWeatherForecast(int id)
        {
            return WeatherForecastService.GetForecastById(id);
        }
    }
}
