using System.Collections.Generic;

namespace graphQLsample
{
    public class GraphQLRequest
    {
        public string? Query { get; set; }
        public string? OperationName { get; set; }
        public Dictionary<string, object>? Variables { get; set; }
    }

    public class GraphQLQueryRequest
    {
        /// <summary>
        /// Raw GraphQL query string
        /// Examples:
        /// - "weatherForecasts"
        /// - "weatherForecast(id: 1)"
        /// - "query { weatherForecasts { id owner date temperatureC temperatureF summary } }"
        /// </summary>
        public string? Query { get; set; }
    }
}
