@echo off
REM GraphQL Query Input Testing - cURL Examples
REM Windows Batch Script

setlocal enabledelayedexpansion

set PORT=7002
set PROTOCOL=https
set ENDPOINT=%PROTOCOL%://localhost:%PORT%/api/graphql/execute

echo.
echo ========================================
echo GraphQL Query Input Testing
echo ========================================
echo Endpoint: %ENDPOINT%
echo.

REM Test 1: Simple Query - Get All Forecasts
echo.
echo ========================================
echo Test 1: Simple Query - Get All Forecasts
echo ========================================
echo Sending: {"query": "weatherForecasts"}
echo.
curl -X POST %ENDPOINT% ^
  -H "Content-Type: application/json" ^
  -d "{\"query\": \"weatherForecasts\"}" ^
  -k

echo.
echo.

REM Test 2: Query with ID Parameter
echo ========================================
echo Test 2: Query with ID Parameter
echo ========================================
echo Sending: {"query": "weatherForecast(id: 1)"}
echo.
curl -X POST %ENDPOINT% ^
  -H "Content-Type: application/json" ^
  -d "{\"query\": \"weatherForecast(id: 1)\"}" ^
  -k

echo.
echo.

REM Test 3: Full GraphQL Query
echo ========================================
echo Test 3: Full GraphQL Query Format
echo ========================================
echo Sending: Full GraphQL query for all forecasts
echo.
curl -X POST %ENDPOINT% ^
  -H "Content-Type: application/json" ^
  -d "{\"query\": \"query { weatherForecasts { id owner date temperatureC temperatureF summary } }\"}" ^
  -k

echo.
echo.

REM Test 4: Query with Operation Name
echo ========================================
echo Test 4: Query with Operation Name
echo ========================================
echo Sending: Named query operation
echo.
curl -X POST %ENDPOINT% ^
  -H "Content-Type: application/json" ^
  -d "{\"query\": \"query GetForecasts { weatherForecasts { id owner summary } }\"}" ^
  -k

echo.
echo.

REM Test 5: Invalid Query
echo ========================================
echo Test 5: Invalid Query (Error Handling)
echo ========================================
echo Sending: {"query": "invalid"}
echo.
curl -X POST %ENDPOINT% ^
  -H "Content-Type: application/json" ^
  -d "{\"query\": \"invalid\"}" ^
  -k

echo.
echo.

echo ========================================
echo Testing Complete!
echo ========================================
echo.
