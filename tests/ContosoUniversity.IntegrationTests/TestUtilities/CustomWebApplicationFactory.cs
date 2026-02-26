using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ContosoUniversity.IntegrationTests.TestUtilities
{
    /// <summary>
    /// Custom WebApplicationFactory for integration testing
    /// Configures in-memory database and test services
    /// </summary>
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the app's SchoolContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<SchoolContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add SchoolContext using an in-memory database for testing
                services.AddDbContext<SchoolContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDb");
                });

                // Build the service provider
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database context
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<SchoolContext>();

                    // Ensure the database is created
                    db.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with test data
                        SeedTestData(db);
                    }
                    catch (Exception ex)
                    {
                        // Log errors during seeding
                        Console.WriteLine($"Error seeding test database: {ex.Message}");
                    }
                }
            });
        }

        private void SeedTestData(SchoolContext context)
        {
            // Add test data for integration tests
            if (!context.Departments.Any())
            {
                context.Departments.Add(new Department
                {
                    Name = "Test Department",
                    Budget = 100000,
                    StartDate = DateTime.Today.AddYears(-5),
                    RowVersion = new byte[8]
                });
                context.SaveChanges();
            }

            if (!context.Students.Any())
            {
                context.Students.AddRange(
                    new Student { FirstMidName = "Test", LastName = "Student1", EnrollmentDate = DateTime.Today.AddYears(-1) },
                    new Student { FirstMidName = "Test", LastName = "Student2", EnrollmentDate = DateTime.Today.AddYears(-2) }
                );
                context.SaveChanges();
            }

            if (!context.Courses.Any())
            {
                var department = context.Departments.First();
                context.Courses.Add(new Course
                {
                    CourseID = 1050,
                    Title = "Test Course",
                    Credits = 3,
                    DepartmentID = department.DepartmentID
                });
                context.SaveChanges();
            }
        }
    }
}
