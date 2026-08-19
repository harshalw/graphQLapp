# GraphQL Weather API - Complete Documentation Index

### ? Infrastructure
- **HotChocolate GraphQL** - GraphQL library
- **Swashbuckle Swagger** - Interactive API documentation
- **.NET Core 9** - Latest framework
- **In-Memory Storage** - Quick testing database
- **Language : C#


### GraphQL API with REST Endpoints
A fully functional GraphQL Weather API that accepts requests and sends responses containing all model fields.

### Core Features
- **Query Operations** - Get all forecasts, get by ID
- **Mutation Operations** - Create new forecast
- **Full Model Fields** - All 6 fields returned in every response
- **Execute requered fields** - as per query its gives response
- **Swagger Integration** - Auto-launching documentation
- **Multiple Testing Options** - Swagger UI, cURL, Postman



### Step 1: Start the Application
```
# Clone solution In Visual Studio 2022 and above version
# In Visual Studio, press F5
# OR use terminal:
dotnet run
```

### Step 2: Open Swagger UI
Browser automatically opens to:
```
https://localhost:7002/
```

### Step 3: excute endpoint

### Endpoints Summary
```
?? QUERY ENDPOINTS (Read)
?  ?? GET  /api/graphql/schema  -- showing schema defination
?  ?? GET  /api/graphql/execute -- input raw query
?  ?? POST /api/graphql/query/forecasts -- Mutation
?  ?? POST /api/graphql/query/forecast/{id} -- execute by Id
?
?? MUTATION ENDPOINTS (Write)
?  ?? POST /api/graphql/mutation/add-forecast


### Model Fields
```json
{
  "id": 1,                    // Auto-generated
  "owner": "MY_ID",           // Record owner
  "date": "2025-01-15",       // Forecast date
  "temperatureC": 25,         // Temperature C°
  "temperatureF": 77,         // Temperature F° (calculated)
  "summary": "Mild"           // Weather description
}
```


