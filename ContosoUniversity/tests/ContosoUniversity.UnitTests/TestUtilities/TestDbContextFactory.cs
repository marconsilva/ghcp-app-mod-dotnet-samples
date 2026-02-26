using ContosoUniversity.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace ContosoUniversity.UnitTests.TestUtilities
{
    /// <summary>
    /// Factory for creating in-memory test database contexts
    /// </summary>
    public static class TestDbContextFactory
    {
        /// <summary>
        /// Creates a new in-memory SchoolContext for testing
        /// </summary>
        /// <param name="databaseName">Unique database name for test isolation</param>
        /// <returns>Configured SchoolContext instance</returns>
        public static SchoolContext CreateInMemoryContext(string databaseName = null)
        {
            databaseName ??= Guid.NewGuid().ToString();
            
            var options = new DbContextOptionsBuilder<SchoolContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .EnableSensitiveDataLogging()
                .Options;

            var context = new SchoolContext(options);
            context.Database.EnsureCreated();
            
            return context;
        }

        /// <summary>
        /// Creates a context with seeded test data
        /// </summary>
        public static SchoolContext CreateSeededContext(string databaseName = null)
        {
            var context = CreateInMemoryContext(databaseName);
            SeedTestData(context);
            return context;
        }

        private static void SeedTestData(SchoolContext context)
        {
            // Add test data here if needed
            context.SaveChanges();
        }
    }
}
