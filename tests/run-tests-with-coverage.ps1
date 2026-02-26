# ContosoUniversity Test Execution Script
# This script runs all tests and generates comprehensive coverage reports

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "ContosoUniversity Test Suite" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Change to solution directory
Set-Location $PSScriptRoot\..

# Clean previous test results
Write-Host "Cleaning previous test results..." -ForegroundColor Yellow
if (Test-Path ".\TestResults") {
    Remove-Item ".\TestResults" -Recurse -Force
}
if (Test-Path ".\coverage-report") {
    Remove-Item ".\coverage-report" -Recurse -Force
}

# Restore packages
Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore

# Build solution
Write-Host "Building solution..." -ForegroundColor Yellow
dotnet build --no-restore

# Run all tests with coverage
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Running Tests with Code Coverage" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

dotnet test --no-build `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat="cobertura,json" `
    /p:CoverletOutput="./TestResults/" `
    /p:ExcludeByFile="**/*.g.cs,**/*.designer.cs,**/AssemblyInfo.cs" `
    /p:Exclude="[*.Tests]*,[*.IntegrationTests]*" `
    --logger "console;verbosity=detailed"

# Check if tests passed
if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "? Tests FAILED" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "? All Tests PASSED" -ForegroundColor Green

# Generate HTML coverage report
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Generating Coverage Report" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if reportgenerator is installed
$reportGenInstalled = dotnet tool list -g | Select-String "dotnet-reportgenerator-globaltool"
if (-not $reportGenInstalled) {
    Write-Host "Installing ReportGenerator tool..." -ForegroundColor Yellow
    dotnet tool install -g dotnet-reportgenerator-globaltool
}

# Find all coverage files
$coverageFiles = Get-ChildItem -Path ".\tests" -Filter "coverage.cobertura.xml" -Recurse | Select-Object -ExpandProperty FullName
if ($coverageFiles) {
    $coverageReports = $coverageFiles -join ";"
    
    reportgenerator `
        -reports:$coverageReports `
        -targetdir:".\coverage-report" `
        -reporttypes:"Html;Badges;HtmlSummary;JsonSummary" `
        -title:"ContosoUniversity Test Coverage"
    
    Write-Host ""
    Write-Host "? Coverage report generated at: .\coverage-report\index.html" -ForegroundColor Green
    
    # Display coverage summary
    if (Test-Path ".\coverage-report\Summary.json") {
        $summary = Get-Content ".\coverage-report\Summary.json" | ConvertFrom-Json
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "Coverage Summary" -ForegroundColor Cyan
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "Line Coverage:   $($summary.summary.linecoverage)%" -ForegroundColor $(if ($summary.summary.linecoverage -ge 80) { "Green" } else { "Yellow" })
        Write-Host "Branch Coverage: $($summary.summary.branchcoverage)%" -ForegroundColor $(if ($summary.summary.branchcoverage -ge 70) { "Green" } else { "Yellow" })
        Write-Host "Method Coverage: $($summary.summary.methodcoverage)%" -ForegroundColor $(if ($summary.summary.methodcoverage -ge 80) { "Green" } else { "Yellow" })
    }
    
    # Open report in browser
    Write-Host ""
    $openReport = Read-Host "Open coverage report in browser? (Y/N)"
    if ($openReport -eq "Y" -or $openReport -eq "y") {
        Start-Process ".\coverage-report\index.html"
    }
}
else {
    Write-Host "??  No coverage files found" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Test Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "? Tests completed successfully" -ForegroundColor Green
Write-Host "?? Coverage report available at: .\coverage-report\index.html" -ForegroundColor Cyan
Write-Host ""
