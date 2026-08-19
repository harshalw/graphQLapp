namespace graphQLsample.GraphQL
{
    public class Mutation
    {
        public WeatherForecast AddWeatherForecast(
            string owner,
            DateOnly date,
            int temperatureC,
            string? summary)
        {
            return WeatherForecastService.AddForecast(owner, date, temperatureC, summary);
        }
    }
}
