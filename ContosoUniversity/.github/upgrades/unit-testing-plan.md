# Comprehensive Unit Testing Plan for ContosoUniversity .NET 9.0

## Executive Summary

This document outlines a comprehensive testing strategy for the ContosoUniversity application that has been modernized from .NET Framework 4.8 to .NET 9.0. The plan includes unit tests, integration tests, and automated test coverage to ensure the modernization is successful and all functionality works correctly.

## Testing Objectives

1. **Verify Modernization Success**: Ensure all migrated features work correctly in .NET 9.0
2. **Achieve High Code Coverage**: Target 80%+ code coverage across the solution
3. **Validate Business Logic**: Test all CRUD operations and business rules
4. **Test Modernized Features**: Specifically test areas affected by the migration
5. **Ensure Data Integrity**: Validate Entity Framework Core operations
6. **Security Testing**: Verify file uploads, validation, and authorization

## Test Project Structure

```
ContosoUniversity.sln
??? ContosoUniversity (Main Project)
??? tests/
    ??? ContosoUniversity.UnitTests
    ?   ??? Controllers/
    ?   ?   ??? HomeControllerTests.cs
    ?   ?   ??? StudentsControllerTests.cs
    ?   ?   ??? CoursesControllerTests.cs
    ?   ?   ??? InstructorsControllerTests.cs
    ?   ?   ??? DepartmentsControllerTests.cs
    ?   ?   ??? NotificationsControllerTests.cs
    ?   ??? Services/
    ?   ?   ??? NotificationServiceTests.cs
    ?   ??? Data/
    ?   ?   ??? DbInitializerTests.cs
    ?   ?   ??? SchoolContextTests.cs
    ?   ??? Models/
    ?   ?   ??? StudentTests.cs
    ?   ?   ??? CourseTests.cs
    ?   ?   ??? InstructorTests.cs
    ?   ?   ??? DepartmentTests.cs
    ?   ?   ??? ValidationTests.cs
    ?   ??? Helpers/
    ?   ?   ??? PaginatedListTests.cs
    ?   ??? TestUtilities/
    ?       ??? TestDbContextFactory.cs
    ?       ??? FakeNotificationService.cs
    ?       ??? TestDataBuilder.cs
    ??? ContosoUniversity.IntegrationTests
        ??? Controllers/
        ?   ??? StudentsControllerIntegrationTests.cs
        ?   ??? CoursesControllerIntegrationTests.cs
        ?   ??? InstructorsControllerIntegrationTests.cs
        ?   ??? DepartmentsControllerIntegrationTests.cs
        ??? Data/
        ?   ??? DatabaseIntegrationTests.cs
        ??? TestUtilities/
            ??? CustomWebApplicationFactory.cs
            ??? TestDatabaseFixture.cs
```

## Testing Stack & Tools

### Core Testing Frameworks
- **xUnit** (v2.9.2) - Primary testing framework
- **Moq** (v4.20.73) - Mocking framework
- **FluentAssertions** (v7.0.0) - Assertion library
- **Microsoft.EntityFrameworkCore.InMemory** (v9.0.13) - In-memory database for testing
- **Microsoft.AspNetCore.Mvc.Testing** (v9.0.13) - Integration testing

### Code Coverage Tools
- **Coverlet** (v6.0.2) - Code coverage collector
- **ReportGenerator** (v5.4.2) - Coverage report generation

### Additional Testing Libraries
- **Bogus** (v35.6.1) - Test data generation
- **Microsoft.NET.Test.Sdk** (v17.12.0) - Test SDK
- **AutoFixture** (v4.18.1) - Auto-generate test data

## Detailed Test Coverage Plan

### 1. Controller Tests (Unit Tests)

#### HomeController Tests
- ? Index action returns correct view
- ? About action returns view with enrollment statistics
- ? Contact action returns correct view
- ? Error action returns error view with model
- ? Unauthorized action returns unauthorized view

#### StudentsController Tests
- **Index Action**:
  - ? Returns paginated list of students
  - ? Handles sorting by name (ascending/descending)
  - ? Handles sorting by date (ascending/descending)
  - ? Filters by search string (first name and last name)
  - ? Handles pagination correctly
  - ? Maintains current filter and sort order

- **Details Action**:
  - ? Returns student with enrollments
  - ? Returns BadRequest when id is null
  - ? Returns NotFound when student doesn't exist

- **Create Actions (GET/POST)**:
  - ? GET returns view with default enrollment date
  - ? POST creates student with valid data
  - ? POST sends notification after creation
  - ? POST validates enrollment date range
  - ? POST validates SQL Server datetime limits
  - ? POST returns view with errors on invalid data

- **Edit Actions (GET/POST)**:
  - ? GET returns student for editing
  - ? POST updates student with valid data
  - ? POST sends notification after update
  - ? POST validates enrollment date
  - ? POST handles concurrency conflicts

- **Delete Actions (GET/POST)**:
  - ? GET returns confirmation view
  - ? POST deletes student
  - ? POST sends notification after deletion
  - ? POST handles database errors gracefully

#### CoursesController Tests
- **CRUD Operations**:
  - ? Index displays courses with departments
  - ? Details shows course information
  - ? Create adds new course
  - ? Edit updates existing course
  - ? Delete removes course

- **File Upload Features** (Modernization Critical):
  - ? Validates file type (jpg, jpeg, png, gif, bmp)
  - ? Validates file size (max 5MB)
  - ? Saves file to wwwroot/Uploads/TeachingMaterials
  - ? Uses IWebHostEnvironment.WebRootPath correctly
  - ? Generates unique filenames
  - ? Deletes old files on update
  - ? Handles upload errors gracefully
  - ? Updates TeachingMaterialImagePath correctly

- **Notification Integration**:
  - ? Sends notifications on CREATE
  - ? Sends notifications on UPDATE
  - ? Sends notifications on DELETE

#### InstructorsController Tests
- **CRUD Operations**:
  - ? Index with instructor selection
  - ? Index with course selection
  - ? Details display
  - ? Create with course assignments
  - ? Edit with TryUpdateModelAsync (Modernization Critical)
  - ? Delete with cascade handling

- **Course Assignment Features**:
  - ? PopulateAssignedCourseData works correctly
  - ? UpdateInstructorCourses adds new assignments
  - ? UpdateInstructorCourses removes old assignments
  - ? Handles null selected courses

#### DepartmentsController Tests
- **CRUD Operations**:
  - ? Index displays departments with administrators
  - ? Details shows department info
  - ? Create adds department
  - ? Edit updates department
  - ? Delete removes department

- **Concurrency Handling** (EF Core Critical):
  - ? Detects concurrent updates
  - ? Shows current database values
  - ? Handles deleted records during edit
  - ? RowVersion tracking works correctly

#### NotificationsController Tests
- **API Endpoints**:
  - ? GetNotifications returns pending notifications
  - ? GetNotifications limits to 10 notifications
  - ? GetNotifications handles errors gracefully
  - ? MarkAsRead updates notification status
  - ? Index returns notification dashboard view

### 2. Service Tests

#### NotificationService Tests (MSMQ Migration Critical)
- **MSMQ Integration**:
  - ? Constructor initializes with IConfiguration (DI modernization)
  - ? Creates queue if it doesn't exist
  - ? Sets queue permissions correctly
  - ? SendNotification sends message to queue
  - ? SendNotification handles errors gracefully
  - ? ReceiveNotification reads from queue
  - ? ReceiveNotification handles timeout
  - ? ReceiveNotification deserializes JSON correctly
  - ? GenerateMessage creates correct message text
  - ? Dispose releases resources

### 3. Data Layer Tests

#### SchoolContext Tests
- **Configuration**:
  - ? DateTime properties use datetime2 column type
  - ? Table-per-Hierarchy (TPH) inheritance configured
  - ? Composite keys configured correctly
  - ? Relationships configured properly
  - ? One-to-one relationships work

#### DbInitializer Tests
- ? Initialize creates database if not exists
- ? Initialize seeds data correctly
- ? Initialize is idempotent (can run multiple times)
- ? Initialize handles existing data

### 4. Model Tests

#### Student Model Tests
- ? Validation attributes work
- ? EnrollmentDate range validation
- ? Required field validation
- ? DisplayFormat attributes applied
- ? Navigation properties initialized

#### Course Model Tests
- ? Required fields validation
- ? CourseID validation
- ? Credits validation
- ? Department relationship
- ? Enrollments collection

#### Instructor Model Tests
- ? HireDate validation
- ? OfficeAssignment relationship
- ? CourseAssignments collection
- ? FullName property calculation

#### Department Model Tests
- ? Concurrency token (RowVersion) works
- ? Budget validation
- ? StartDate validation
- ? Administrator relationship

### 5. Helper/Utility Tests

#### PaginatedList Tests
- ? Create method paginates correctly
- ? PageIndex calculated correctly
- ? TotalPages calculated correctly
- ? HasPreviousPage property
- ? HasNextPage property
- ? Handles empty collections
- ? Handles single page collections

### 6. Integration Tests

#### End-to-End Scenarios
- ? Student enrollment workflow
- ? Course creation and assignment
- ? Instructor course assignments
- ? Department administrator management
- ? Notification system end-to-end
- ? File upload and retrieval
- ? Database initialization on startup

#### Modernization-Specific Integration Tests
- ? ASP.NET Core MVC routing works
- ? Dependency injection resolves all services
- ? appsettings.json configuration loads correctly
- ? Static files served from wwwroot
- ? Middleware pipeline functions correctly
- ? IWebHostEnvironment resolves paths correctly

## Test Categories by Priority

### Priority 1: Critical Modernization Tests
These tests validate that the migration from .NET Framework to .NET Core was successful:

1. **Dependency Injection Tests**
   - BaseController receives SchoolContext via DI
   - NotificationService receives IConfiguration via DI
   - All controllers instantiate correctly

2. **Configuration Migration Tests**
   - appsettings.json loads correctly
   - Connection strings retrieved properly
   - NotificationQueuePath configuration works

3. **File System Tests**
   - IWebHostEnvironment.WebRootPath resolves correctly
   - Static files served from wwwroot
   - File uploads work with new path structure

4. **MSMQ Migration Tests**
   - MSMQ.Messaging library works correctly
   - Queue operations function properly
   - Error handling works as expected

5. **Routing Tests**
   - ASP.NET Core MVC routing works
   - Default route configured correctly
   - ActionLink helpers generate correct URLs

6. **API Compatibility Tests**
   - TryUpdateModelAsync replaces TryUpdateModel
   - IFormFile replaces HttpPostedFileBase
   - NotFound(), BadRequest() work correctly
   - StatusCode() methods work

### Priority 2: Business Logic Tests
Standard business logic validation:

1. Student CRUD operations
2. Course CRUD operations
3. Instructor CRUD operations
4. Department CRUD operations with concurrency
5. Enrollment management
6. Pagination and sorting
7. Search functionality

### Priority 3: Data Layer Tests
Entity Framework Core specific tests:

1. DbContext configuration
2. Entity relationships
3. Migrations
4. Query performance
5. Transaction handling

## Code Coverage Targets

### Overall Coverage Goal: 80%+

| Component              | Target Coverage | Priority |
|:-----------------------|:---------------:|:--------:|
| Controllers            | 85%             | High     |
| Services               | 90%             | High     |
| Models                 | 75%             | Medium   |
| Data Layer             | 85%             | High     |
| Helpers/Utilities      | 90%             | High     |
| Program.cs             | 50%             | Low      |

## Testing Best Practices

### 1. Arrange-Act-Assert (AAA) Pattern
```csharp
[Fact]
public void Create_ValidStudent_AddsToDatabase()
{
    // Arrange
    var student = new Student { /* ... */ };
    var controller = new StudentsController(/* ... */);
    
    // Act
    var result = controller.Create(student);
    
    // Assert
    result.Should().BeOfType<RedirectToActionResult>();
}
```

### 2. Test Naming Convention
Use descriptive names: `MethodName_Scenario_ExpectedBehavior`

Examples:
- `Create_ValidStudent_ReturnsRedirectToIndex`
- `Create_InvalidDate_ReturnsViewWithError`
- `Index_WithSearchString_FiltersStudents`

### 3. Use In-Memory Database for Unit Tests
```csharp
var options = new DbContextOptionsBuilder<SchoolContext>()
    .UseInMemoryDatabase(databaseName: "TestDatabase")
    .Options;
```

### 4. Mock External Dependencies
- Mock IConfiguration for configuration values
- Mock IWebHostEnvironment for file operations
- Mock MSMQ MessageQueue for notification tests

### 5. Test Data Builders
Create reusable test data builders for consistent test data:
```csharp
public class StudentBuilder
{
    private Student _student = new Student
    {
        FirstMidName = "Test",
        LastName = "Student",
        EnrollmentDate = DateTime.Today
    };
    
    public StudentBuilder WithEnrollmentDate(DateTime date)
    {
        _student.EnrollmentDate = date;
        return this;
    }
    
    public Student Build() => _student;
}
```

## Implementation Steps

### Phase 1: Setup Test Infrastructure (Week 1)
1. Create test projects
2. Add NuGet packages
3. Create test utilities and base classes
4. Set up in-memory database helpers
5. Create mock factories
6. Set up code coverage tools

### Phase 2: Critical Modernization Tests (Week 1-2)
Focus on testing areas specifically affected by the .NET 9.0 migration:
1. Dependency injection tests
2. Configuration migration tests
3. File system and wwwroot tests
4. MSMQ migration tests
5. Routing tests
6. API compatibility tests

### Phase 3: Controller Unit Tests (Week 2-3)
1. HomeController tests
2. StudentsController tests (all CRUD + pagination)
3. CoursesController tests (all CRUD + file upload)
4. InstructorsController tests (all CRUD + assignments)
5. DepartmentsController tests (all CRUD + concurrency)
6. NotificationsController tests

### Phase 4: Service & Data Layer Tests (Week 3)
1. NotificationService tests
2. SchoolContext tests
3. DbInitializer tests
4. Model validation tests

### Phase 5: Integration Tests (Week 4)
1. End-to-end workflow tests
2. Database integration tests
3. Full stack feature tests

### Phase 6: Code Coverage Analysis (Week 4)
1. Generate coverage reports
2. Identify gaps
3. Add missing tests
4. Achieve 80%+ coverage target

## Test Execution Strategy

### Local Development
```bash
# Run all tests
dotnet test

# Run with code coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Generate HTML coverage report
reportgenerator -reports:coverage.cobertura.xml -targetdir:coverage-report
```

### CI/CD Integration
- Run tests on every pull request
- Block merges if tests fail
- Block merges if coverage drops below 80%
- Generate and publish coverage reports

## Key Testing Scenarios by Modernized Feature

### 1. Bundling & Minification Migration
**What Changed**: Removed System.Web.Optimization, now using direct script/link tags

**Tests Needed**:
- ? Verify static files served from wwwroot
- ? Check script paths resolve correctly
- ? Validate CSS paths resolve correctly
- ? Test that all views render without missing resources

### 2. Routing Migration
**What Changed**: RouteCollection ? ASP.NET Core routing in Program.cs

**Tests Needed**:
- ? Default route {controller=Home}/{action=Index}/{id?} works
- ? All ActionLink helpers generate correct URLs
- ? RedirectToAction works correctly
- ? Route parameters parsed correctly

### 3. System.Messaging ? MSMQ.Messaging
**What Changed**: Migrated to MSMQ.Messaging with dependency injection

**Tests Needed**:
- ? Queue creation with IConfiguration
- ? Message sending works
- ? Message receiving works
- ? JSON serialization/deserialization
- ? Error handling without breaking main operations
- ? Dispose pattern works correctly

### 4. Global.asax.cs ? Program.cs
**What Changed**: Application initialization moved to Program.cs

**Tests Needed**:
- ? Database initialization runs on startup
- ? Services registered correctly
- ? Middleware pipeline configured correctly
- ? Exception handling middleware works
- ? Static files middleware works

### 5. File Uploads (HttpPostedFileBase ? IFormFile)
**What Changed**: Migrated to ASP.NET Core IFormFile

**Tests Needed**:
- ? File upload with IFormFile works
- ? File validation (type and size) works
- ? File saving to wwwroot works
- ? Path resolution with IWebHostEnvironment works
- ? Old file deletion on update works
- ? Error handling works

### 6. Model Binding (TryUpdateModel ? TryUpdateModelAsync)
**What Changed**: Updated to async version with expression-based properties

**Tests Needed**:
- ? TryUpdateModelAsync binds properties correctly
- ? Async operation completes successfully
- ? ModelState validation works
- ? Partial updates work correctly

## Coverage Metrics & Reporting

### Coverage Report Components
1. **Line Coverage**: Percentage of code lines executed
2. **Branch Coverage**: Percentage of decision branches tested
3. **Method Coverage**: Percentage of methods called
4. **Cyclomatic Complexity**: Code complexity metrics

### Report Formats
- **HTML Report**: Visual coverage report with drill-down
- **Cobertura XML**: For CI/CD integration
- **JSON**: For programmatic analysis
- **Console Summary**: Quick overview

### Coverage Exclusions
Exclude from coverage analysis:
- Auto-generated files (*.g.cs, *.designer.cs)
- Program.cs (startup code)
- AssemblyInfo.cs
- Migration files
- View models without logic

## Mock Strategy

### Controllers - Mock Dependencies
```csharp
Mock<SchoolContext> mockContext
Mock<NotificationService> mockNotificationService
Mock<IWebHostEnvironment> mockEnvironment
```

### Services - Mock Dependencies
```csharp
Mock<IConfiguration> mockConfiguration
Mock<MessageQueue> mockQueue (if testable)
```

### Use Real In-Memory Database
For integration-like unit tests, use EF Core InMemory database to test actual queries.

## Continuous Improvement

### Code Review Checklist
- [ ] All new code has corresponding unit tests
- [ ] Tests follow AAA pattern
- [ ] Test names are descriptive
- [ ] No hardcoded values in tests
- [ ] Mocks used appropriately
- [ ] Integration tests cover critical paths

### Monitoring & Maintenance
- Review coverage reports weekly
- Add tests for bug fixes
- Update tests when refactoring
- Keep test dependencies updated

## Success Criteria

### Definition of Done
- ? All test projects created and building
- ? 80%+ overall code coverage achieved
- ? All controllers have comprehensive tests
- ? All services have unit tests
- ? Critical modernization paths tested
- ? Integration tests cover main workflows
- ? All tests passing consistently
- ? Coverage reports generated and accessible
- ? CI/CD pipeline includes test execution

### Quality Gates
- **Build**: Must succeed
- **Tests**: 100% pass rate
- **Coverage**: Minimum 80%
- **Performance**: Test suite completes in < 5 minutes

## Estimated Effort

| Phase                              | Estimated Time | Resources |
|:-----------------------------------|:--------------:|:---------:|
| Setup Test Infrastructure          | 4-8 hours      | 1 dev     |
| Critical Modernization Tests       | 16-24 hours    | 1-2 devs  |
| Controller Unit Tests              | 24-32 hours    | 2 devs    |
| Service & Data Layer Tests         | 16-20 hours    | 1-2 devs  |
| Integration Tests                  | 16-20 hours    | 1-2 devs  |
| Code Coverage Analysis & Cleanup   | 8-12 hours     | 1 dev     |
| **Total**                          | **84-116 hours**| **2-3 weeks** |

## Appendix: Sample Test Templates

### Controller Test Template
```csharp
public class StudentsControllerTests
{
    private readonly Mock<SchoolContext> _mockContext;
    private readonly Mock<NotificationService> _mockNotificationService;
    private readonly StudentsController _controller;
    
    public StudentsControllerTests()
    {
        _mockContext = new Mock<SchoolContext>();
        _mockNotificationService = new Mock<NotificationService>();
        _controller = new StudentsController(
            _mockContext.Object, 
            _mockNotificationService.Object);
    }
    
    [Fact]
    public void Index_ReturnsViewWithStudents()
    {
        // Test implementation
    }
}
```

### Integration Test Template
```csharp
public class StudentsControllerIntegrationTests : 
    IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public StudentsControllerIntegrationTests(
        CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task Index_ReturnsSuccessStatusCode()
    {
        // Test implementation
    }
}
```

## Next Steps

1. Review and approve this testing plan
2. Create test projects structure
3. Begin Phase 1: Setup Test Infrastructure
4. Implement tests iteratively by phase
5. Monitor coverage and adjust as needed

---

**Plan Version**: 1.0  
**Created**: February 26, 2026  
**Status**: Ready for Implementation
