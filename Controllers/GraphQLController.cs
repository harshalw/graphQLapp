using Microsoft.AspNetCore.Mvc;

namespace graphQLsample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GraphQLController : ControllerBase
    {
        /// <summary>
        /// Execute GraphQL query - Get all weather forecasts
        /// </summary>
        /// <returns>List of all weather forecasts with all model fields</returns>
        [HttpPost("query/forecasts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllForecasts()
        {
            try
            {
                var forecasts = WeatherForecastService.GetAllForecasts();

                var response = new
                {
                    query = "weatherForecasts",
                    data = forecasts.Select(f => new
                    {
                        id = f.Id,
                        owner = f.Owner,
                        date = f.Date.ToString("yyyy-MM-dd"),
                        temperatureC = f.TemperatureC,
                        temperatureF = f.TemperatureF,
                        summary = f.Summary
                    }),
                    errors = (string[]?)null
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    query = "weatherForecasts",
                    data = (object?)null,
                    errors = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Execute GraphQL query - Get forecast by ID
        /// </summary>
        /// <param name="id">The forecast ID</param>
        /// <returns>Weather forecast details with all model fields</returns>
        [HttpPost("query/forecast/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetForecastById(int id)
        {
            try
            {
                var forecast = WeatherForecastService.GetForecastById(id);

                if (forecast == null)
                {
                    return NotFound(new
                    {
                        query = $"weatherForecast(id: {id})",
                        data = (object?)null,
                        errors = new[] { $"Forecast with ID {id} not found" }
                    });
                }

                var response = new
                {
                    query = $"weatherForecast(id: {id})",
                    data = new
                    {
                        id = forecast.Id,
                        owner = forecast.Owner,
                        date = forecast.Date.ToString("yyyy-MM-dd"),
                        temperatureC = forecast.TemperatureC,
                        temperatureF = forecast.TemperatureF,
                        summary = forecast.Summary
                    },
                    errors = (string[]?)null
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    query = $"weatherForecast(id: {id})",
                    data = (object?)null,
                    errors = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Execute GraphQL mutation - Add new forecast
        /// </summary>
        /// <param name="request">Mutation request with owner, date, temperatureC, summary</param>
        /// <returns>The newly created forecast with all model fields</returns>
        [HttpPost("mutation/add-forecast")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddForecast([FromBody] AddForecastRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Owner))
                {
                    return BadRequest(new
                    {
                        mutation = "addWeatherForecast",
                        data = (object?)null,
                        errors = new[] { "Owner is required" }
                    });
                }

                var forecast = WeatherForecastService.AddForecast(
                    request.Owner,
                    request.Date,
                    request.TemperatureC,
                    request.Summary);

                var response = new
                {
                    mutation = "addWeatherForecast",
                    data = new
                    {
                        id = forecast.Id,
                        owner = forecast.Owner,
                        date = forecast.Date.ToString("yyyy-MM-dd"),
                        temperatureC = forecast.TemperatureC,
                        temperatureF = forecast.TemperatureF,
                        summary = forecast.Summary
                    },
                    errors = (string[]?)null
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mutation = "addWeatherForecast",
                    data = (object?)null,
                    errors = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Execute raw GraphQL query string
        /// </summary>
        /// <param name="request">GraphQL request with query string</param>
        /// <returns>GraphQL response with data or errors</returns>
        [HttpPost("execute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ExecuteGraphQLQuery([FromBody] GraphQLQueryRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Query))
                {
                    return BadRequest(new
                    {
                        query = request.Query,
                        data = (object?)null,
                        errors = new[] { "Query is required" }
                    });
                }

                // Parse requested fields from the GraphQL query
                var requestedFields = ExtractRequestedFields(request.Query);

                // Parse the query string to determine operation type and fields
                var queryLower = request.Query.ToLower().Trim();

                // Check if it's a query for all forecasts
                if (queryLower.Contains("weatherforecasts"))
                {
                    var forecasts = WeatherForecastService.GetAllForecasts();

                    var response = new
                    {
                        query = request.Query,
                        data = forecasts.Select(f => BuildForecastObject(f, requestedFields)),
                        errors = (string[]?)null
                    };

                    return Ok(response);
                }
                // Check if it's a query for forecast by ID
                else if (queryLower.Contains("weatherforecast") && queryLower.Contains("id"))
                {
                    // Extract ID from query (e.g., "weatherForecast(id: 1)")
                    int? id = ExtractIdFromQuery(request.Query);

                    if (id == null)
                    {
                        return BadRequest(new
                        {
                            query = request.Query,
                            data = (object?)null,
                            errors = new[] { "Could not parse ID from query" }
                        });
                    }

                    var forecast = WeatherForecastService.GetForecastById(id.Value);

                    if (forecast == null)
                    {
                        return NotFound(new
                        {
                            query = request.Query,
                            data = (object?)null,
                            errors = new[] { $"Forecast with ID {id} not found" }
                        });
                    }

                    var response = new
                    {
                        query = request.Query,
                        data = BuildForecastObject(forecast, requestedFields),
                        errors = (string[]?)null
                    };

                    return Ok(response);
                }
                // Check if it's a mutation to add forecast
                else if (queryLower.Contains("addweatherforecast") || queryLower.Contains("mutation"))
                {
                    return BadRequest(new
                    {
                        query = request.Query,
                        data = (object?)null,
                        errors = new[] { "For mutations, use the /api/graphql/mutation/add-forecast endpoint with JSON body" }
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        query = request.Query,
                        data = (object?)null,
                        errors = new[] { "Query not recognized. Use 'weatherForecasts' or 'weatherForecast(id: N)'" }
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    query = request.Query,
                    data = (object?)null,
                    errors = new[] { $"Error executing query: {ex.Message}" }
                });
            }
        }

        /// <summary>
        /// Helper method to extract ID from GraphQL query
        /// </summary>
        private int? ExtractIdFromQuery(string query)
        {
            try
            {
                // Look for pattern like "id: 1" or "id:1"
                var match = System.Text.RegularExpressions.Regex.Match(query, @"id\s*:\s*(\d+)");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int id))
                {
                    return id;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get schema information showing all available model fields
        /// </summary>
        /// <returns>GraphQL schema with model field details</returns>
        [HttpGet("schema")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetSchema()
        {
            var schema = new
            {
                modelFields = new
                {
                    WeatherForecast = new
                    {
                        id = new { type = "Int", description = "Unique identifier", required = true },
                        owner = new { type = "String", description = "Record owner identifier", required = true },
                        date = new { type = "Date", description = "Date of the forecast (YYYY-MM-DD)", required = true },
                        temperatureC = new { type = "Int", description = "Temperature in Celsius", required = true },
                        temperatureF = new { type = "Int", description = "Temperature in Fahrenheit (calculated)", required = false },
                        summary = new { type = "String", description = "Weather summary", required = false }
                    }
                },
                queries = new
                {
                    weatherForecasts = new
                    {
                        description = "Get all weather forecasts",
                        endpoint = "POST /api/graphql/query/forecasts",
                        returnFields = new[] { "id", "owner", "date", "temperatureC", "temperatureF", "summary" }
                    },
                    weatherForecast = new
                    {
                        description = "Get weather forecast by ID",
                        endpoint = "POST /api/graphql/query/forecast/{id}",
                        parameters = new { id = "Int (required)" },
                        returnFields = new[] { "id", "owner", "date", "temperatureC", "temperatureF", "summary" }
                    }
                },
                mutations = new
                {
                    addWeatherForecast = new
                    {
                        description = "Add a new weather forecast",
                        endpoint = "POST /api/graphql/mutation/add-forecast",
                        inputFields = new
                        {
                            owner = new { type = "String", description = "Owner identifier", required = true },
                            date = new { type = "Date", description = "Forecast date (YYYY-MM-DD)", required = true },
                            temperatureC = new { type = "Int", description = "Temperature in Celsius", required = true },
                            summary = new { type = "String", description = "Weather summary", required = false }
                        },
                        returnFields = new[] { "id", "owner", "date", "temperatureC", "temperatureF", "summary" }
                    }
                }
            };

            return Ok(schema);
        }

        /// <summary>
        /// Get example requests and responses
        /// </summary>
        /// <returns>Sample GraphQL requests and expected responses</returns>
        [HttpGet("examples")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetExamples()
        {
            var examples = new
            {
                queries = new
                {
                    getAllForecasts = new
                    {
                        endpoint = "POST /api/graphql/query/forecasts",
                        description = "Fetch all weather forecasts",
                        request = new
                        {
                            query = "weatherForecasts",
                            operation = "query"
                        },
                        responseExample = new
                        {
                            query = "weatherForecasts",
                            data = new[] {
                                new {
                                    id = 1,
                                    owner = "MY_ID",
                                    date = "2025-01-15",
                                    temperatureC = 25,
                                    temperatureF = 77,
                                    summary = "Mild"
                                }
                            },
                            errors = (string[]?)null
                        }
                    },
                    getForecastById = new
                    {
                        endpoint = "POST /api/graphql/query/forecast/1",
                        description = "Fetch a specific forecast by ID",
                        urlParameter = "id",
                        responseExample = new
                        {
                            query = "weatherForecast(id: 1)",
                            data = new
                            {
                                id = 1,
                                owner = "MY_ID",
                                date = "2025-01-15",
                                temperatureC = 25,
                                temperatureF = 77,
                                summary = "Mild"
                            },
                            errors = (string[]?)null
                        }
                    }
                },
                mutations = new
                {
                    addForecast = new
                    {
                        endpoint = "POST /api/graphql/mutation/add-forecast",
                        description = "Add a new weather forecast",
                        requestExample = new
                        {
                            owner = "user1",
                            date = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                            temperatureC = 15,
                            summary = "Cloudy"
                        },
                        responseExample = new
                        {
                            mutation = "addWeatherForecast",
                            data = new
                            {
                                id = 2,
                                owner = "user1",
                                date = DateOnly.FromDateTime(DateTime.Now.AddDays(5)).ToString("yyyy-MM-dd"),
                                temperatureC = 15,
                                temperatureF = 59,
                                summary = "Cloudy"
                            },
                            errors = (string[]?)null
                        }
                    }
                }
            };

            return Ok(examples);
        }

        /// <summary>
        /// Helper method to extract requested fields from GraphQL query
        /// </summary>
        private HashSet<string> ExtractRequestedFields(string query)
        {
            var fields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                // Find the content between curly braces
                var openBraceIndex = query.IndexOf('{');
                var closeBraceIndex = query.LastIndexOf('}');

                if (openBraceIndex >= 0 && closeBraceIndex > openBraceIndex)
                {
                    var content = query.Substring(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 1);

                    // Split by common delimiters and extract field names
                    var fieldNames = new[] { "id", "owner", "date", "temperatureC", "temperatureF", "summary" };

                    foreach (var field in fieldNames)
                    {
                        if (content.Contains(field, StringComparison.OrdinalIgnoreCase))
                        {
                            fields.Add(field.ToLower());
                        }
                    }
                }

                // If no fields extracted, return all fields
                if (fields.Count == 0)
                {
                    return new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "id", "owner", "date", "temperatureC", "temperatureF", "summary" };
                }
            }
            catch
            {
                // If parsing fails, return all fields
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "id", "owner", "date", "temperatureC", "temperatureF", "summary" };
            }

            return fields;
        }

        /// <summary>
        /// Helper method to build forecast object with only requested fields
        /// </summary>
        private dynamic BuildForecastObject(WeatherForecast forecast, HashSet<string> requestedFields)
        {
            var obj = new System.Collections.Generic.Dictionary<string, object?>();

            if (requestedFields.Contains("id"))
                obj["id"] = forecast.Id;

            if (requestedFields.Contains("owner"))
                obj["owner"] = forecast.Owner;

            if (requestedFields.Contains("date"))
                obj["date"] = forecast.Date.ToString("yyyy-MM-dd");

            if (requestedFields.Contains("temperaturec"))
                obj["temperatureC"] = forecast.TemperatureC;

            if (requestedFields.Contains("temperaturef"))
                obj["temperatureF"] = forecast.TemperatureF;

            if (requestedFields.Contains("summary"))
                obj["summary"] = forecast.Summary;

            return obj;
        }
    }

    public class AddForecastRequest
    {
        public string Owner { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public int TemperatureC { get; set; }
        public string? Summary { get; set; }
    }
}
