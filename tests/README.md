# ContosoUniversity Test Suite

## Overview

This test suite provides comprehensive coverage for the ContosoUniversity application that has been modernized from .NET Framework 4.8 to .NET 9.0.

## Test Projects

### ContosoUniversity.UnitTests
Fast-running isolated unit tests for controllers, services, models, and data layer.

**Key Technologies:**
- xUnit 2.9.2
- Moq 4.20.72
- FluentAssertions 7.0.0
- EF Core InMemory 9.0.13
- Bogus 35.6.1 (test data generation)
- Coverlet 6.0.2 (code coverage)

### ContosoUniversity.IntegrationTests
End-to-end integration tests validating the complete application stack.

**Key Technologies:**
- xUnit 2.9.2
- Microsoft.AspNetCore.Mvc.Testing 9.0.13
- FluentAssertions 7.0.0
- EF Core InMemory 9.0.13

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Only Unit Tests
```bash
dotnet test --filter "FullyQualifiedName~UnitTests"
```

### Run Only Integration Tests
```bash
dotnet test --filter "FullyQualifiedName~IntegrationTests"
```

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~StudentsControllerTests"
```

### Run Tests with Code Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/
```

### Generate HTML Coverage Report
```bash
# Install ReportGenerator if not already installed
dotnet tool install -g dotnet-reportgenerator-globaltool

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/coverage.cobertura.xml

# Generate HTML report
reportgenerator -reports:./tests/**/TestResults/coverage.cobertura.xml -targetdir:./coverage-report -reporttypes:Html

# Open report
start coverage-report/index.html
```

### Run Tests in Watch Mode (Development)
```bash
dotnet watch test --project tests/ContosoUniversity.UnitTests
```

## Test Coverage

### Current Coverage Status

| Component              | Line Coverage | Branch Coverage | Status |
|:-----------------------|:-------------:|:---------------:|:------:|
| Controllers            | TBD           | TBD             | ??     |
| Services               | TBD           | TBD             | ??     |
| Models                 | TBD           | TBD             | ??     |
| Data Layer             | TBD           | TBD             | ??     |
| Helpers                | TBD           | TBD             | ??     |
| **Overall**            | **TBD**       | **TBD**         | ??     |

**Target**: 80%+ line coverage, 70%+ branch coverage

## Test Organization

### Test Naming Convention
Tests follow the pattern: `MethodName_Scenario_ExpectedBehavior`

Examples:
- `Create_ValidStudent_AddsToDatabase`
- `Index_WithSearchString_FiltersStudents`
- `Edit_ConcurrentUpdate_DetectsConflict`

### Test Categories

#### Modernization Tests ??
Tests that specifically validate the .NET Framework ? .NET 9.0 migration:
- Dependency injection in controllers
- IFormFile instead of HttpPostedFileBase
- IWebHostEnvironment instead of Server.MapPath
- TryUpdateModelAsync instead of TryUpdateModel
- IConfiguration instead of ConfigurationManager
- MSMQ.Messaging instead of System.Messaging
- wwwroot static file serving
- ASP.NET Core routing
- Program.cs initialization

#### Business Logic Tests ??
Tests for core application functionality:
- CRUD operations for all entities
- Pagination and sorting
- Search and filtering
- Data validation
- Business rules

#### Data Layer Tests ??
Tests for Entity Framework Core:
- DbContext configuration
- Entity relationships
- Concurrency handling
- Query operations
- DateTime2 column types

#### Integration Tests ??
End-to-end tests:
- HTTP request/response workflows
- Routing validation
- Static file serving
- Database operations
- Error handling

## Key Test Files

### Critical Modernization Tests

| Test File | Focus Area | Priority |
|:----------|:-----------|:--------:|
| `CoursesControllerTests.cs` | IFormFile upload, IWebHostEnvironment | ??? |
| `NotificationServiceTests.cs` | MSMQ migration, IConfiguration DI | ??? |
| `StudentsControllerTests.cs` | Controller DI, pagination | ??? |
| `DepartmentsControllerTests.cs` | EF Core concurrency | ??? |
| `SchoolContextTests.cs` | EF Core config, datetime2 | ??? |
| `StudentsControllerIntegrationTests.cs` | Routing, static files, Program.cs | ??? |

## Continuous Integration

### GitHub Actions Example
```yaml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: windows-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 9.0.x
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test with Coverage
      run: dotnet test --no-build --verbosity normal /p:CollectCoverage=true
    - name: Upload Coverage
      uses: codecov/codecov-action@v3
```

## Troubleshooting

### Common Issues

#### MSMQ Tests Failing
If MSMQ tests fail, ensure Message Queuing is enabled on Windows:
```powershell
Enable-WindowsOptionalFeature -Online -FeatureName MSMQ-Server -All
```

#### In-Memory Database Issues
Each test uses a unique database name to prevent cross-test contamination.

#### File Upload Tests
Tests create temporary directories that are cleaned up after execution.

## Contributing

When adding new features:
1. Write tests first (TDD approach recommended)
2. Ensure new code has >80% coverage
3. Add both unit and integration tests
4. Update this README if new test patterns are introduced

## Next Steps

1. ? Test infrastructure created
2. ? Sample tests implemented
3. ?? Run tests and verify they pass
4. ?? Complete remaining controller tests
5. ?? Add more integration tests
6. ?? Generate initial coverage report
7. ?? Fill coverage gaps
8. ?? Achieve 80%+ coverage target

## Resources

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [EF Core Testing](https://learn.microsoft.com/en-us/ef/core/testing/)
- [ASP.NET Core Integration Tests](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)

---

**Last Updated**: February 26, 2026  
**Test Framework**: xUnit 2.9.2  
**Target Coverage**: 80%+
