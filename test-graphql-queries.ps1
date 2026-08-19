#!/usr/bin/env powershell
# GraphQL Query Input Testing Script
# Run: .\test-graphql-queries.ps1

# Color output
function Write-Header {
    param([string]$Text)
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host $Text -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Text)
    Write-Host $Text -ForegroundColor Green
}

function Write-Error-Custom {
    param([string]$Text)
    Write-Host $Text -ForegroundColor Red
}

# Configuration
$port = 7002
$protocol = "https"
$endpoint = "$protocol://localhost:$port/api/graphql/execute"

Write-Header "GraphQL Query Input Testing"
Write-Host "Endpoint: $endpoint" -ForegroundColor Yellow

# Test 1: Simple Query - Get All Forecasts
Write-Header "Test 1: Simple Query - Get All Forecasts"
Write-Host 'Sending: {"query": "weatherForecasts"}' -ForegroundColor Yellow

try {
    $body = @{
        query = "weatherForecasts"
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri $endpoint `
        -Method Post `
        -ContentType "application/json" `
        -Body $body `
        -SkipCertificateCheck

    Write-Success "? Success!"
    Write-Host ($response | ConvertTo-Json -Depth 10) -ForegroundColor Green
}
catch {
    Write-Error-Custom "? Failed: $_"
}

# Test 2: Query with ID Parameter
Write-Header "Test 2: Query with ID Parameter"
Write-Host 'Sending: {"query": "weatherForecast(id: 1)"}' -ForegroundColor Yellow

try {
    $body = @{
        query = "weatherForecast(id: 1)"
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri $endpoint `
        -Method Post `
        -ContentType "application/json" `
        -Body $body `
        -SkipCertificateCheck

    Write-Success "? Success!"
    Write-Host ($response | ConvertTo-Json -Depth 10) -ForegroundColor Green
}
catch {
    Write-Error-Custom "? Failed: $_"
}

# Test 3: Full GraphQL Query Format
Write-Header "Test 3: Full GraphQL Query Format"
$graphqlQuery = @"
query {
  weatherForecasts {
    id
    owner
    date
    temperatureC
    temperatureF
    summary
  }
}
"@

Write-Host "Sending: Full GraphQL query" -ForegroundColor Yellow

try {
    $body = @{
        query = $graphqlQuery
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri $endpoint `
        -Method Post `
        -ContentType "application/json" `
        -Body $body `
        -SkipCertificateCheck

    Write-Success "? Success!"
    Write-Host ($response | ConvertTo-Json -Depth 10) -ForegroundColor Green
}
catch {
    Write-Error-Custom "? Failed: $_"
}

# Test 4: Query Specific Forecast with Full GraphQL
Write-Header "Test 4: Query Specific Forecast with Full GraphQL"
$graphqlQuery = @"
query GetForecast {
  weatherForecast(id: 1) {
    id
    owner
    date
    temperatureC
    temperatureF
    summary
  }
}
"@

Write-Host "Sending: Query specific forecast" -ForegroundColor Yellow

try {
    $body = @{
        query = $graphqlQuery
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri $endpoint `
        -Method Post `
        -ContentType "application/json" `
        -Body $body `
        -SkipCertificateCheck

    Write-Success "? Success!"
    Write-Host ($response | ConvertTo-Json -Depth 10) -ForegroundColor Green
}
catch {
    Write-Error-Custom "? Failed: $_"
}

# Test 5: Invalid Query (Error Handling)
Write-Header "Test 5: Invalid Query (Error Handling)"
Write-Host 'Sending: {"query": "invalid"}' -ForegroundColor Yellow

try {
    $body = @{
        query = "invalid"
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri $endpoint `
        -Method Post `
        -ContentType "application/json" `
        -Body $body `
        -SkipCertificateCheck

    Write-Host ($response | ConvertTo-Json -Depth 10) -ForegroundColor Yellow
    Write-Host "Expected error returned" -ForegroundColor Yellow
}
catch {
    Write-Error-Custom "? Failed: $_"
}

# Test 6: Empty Query (Error Handling)
Write-Header "Test 6: Empty Query (Error Handling)"
Write-Host 'Sending: {"query": ""}' -ForegroundColor Yellow

try {
    $body = @{
        query = ""
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri $endpoint `
        -Method Post `
        -ContentType "application/json" `
        -Body $body `
        -SkipCertificateCheck

    Write-Host ($response | ConvertTo-Json -Depth 10) -ForegroundColor Yellow
    Write-Host "Expected error returned" -ForegroundColor Yellow
}
catch {
    Write-Error-Custom "? Failed: $_"
}

Write-Header "Testing Complete!"
Write-Host "All tests have been executed. Check results above." -ForegroundColor Cyan
