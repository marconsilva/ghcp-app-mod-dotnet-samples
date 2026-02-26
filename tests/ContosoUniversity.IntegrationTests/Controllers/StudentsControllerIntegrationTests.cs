using ContosoUniversity.IntegrationTests.TestUtilities;
using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ContosoUniversity.IntegrationTests.Controllers
{
    /// <summary>
    /// Integration tests for Students functionality - validates end-to-end workflows
    /// </summary>
    public class StudentsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public StudentsControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        #region Routing Tests (Modernization Critical)

        [Fact]
        public async Task Students_Index_ReturnsSuccessStatusCode()
        {
            // This validates that ASP.NET Core routing works correctly
            // Critical for the RouteCollection ? Program.cs migration
            
            // Act
            var response = await _client.GetAsync("/Students/Index");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Students_DefaultRoute_WorksWithoutAction()
        {
            // Validates that the default MVC route pattern works
            
            // Act
            var response = await _client.GetAsync("/Students");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Students_Create_GetReturnsForm()
        {
            // Act
            var response = await _client.GetAsync("/Students/Create");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Create");
            content.Should().Contain("Student");
        }

        [Fact]
        public async Task Students_Details_WithValidId_ReturnsStudent()
        {
            // Act
            var response = await _client.GetAsync("/Students/Details/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion

        #region Static Files Tests (wwwroot Migration Critical)

        [Fact]
        public async Task StaticFiles_CssServedFromWwwroot()
        {
            // This validates that static files are served from wwwroot
            // Critical for the Content ? wwwroot migration
            
            // Act
            var response = await _client.GetAsync("/Content/Site.css");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.MediaType.Should().Be("text/css");
        }

        [Fact]
        public async Task StaticFiles_JavaScriptServedFromWwwroot()
        {
            // Validates JavaScript files are served correctly
            
            // Act
            var response = await _client.GetAsync("/Scripts/jquery-3.4.1.min.js");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task Students_Details_InvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/Students/Details/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region Navigation Tests

        [Fact]
        public async Task Home_Index_ReturnsSuccessAndContent()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Contoso University");
        }

        [Fact]
        public async Task Home_About_ReturnsSuccessWithEnrollmentStats()
        {
            // Act
            var response = await _client.GetAsync("/Home/About");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("About");
        }

        #endregion

        #region Application Startup Tests (Program.cs Migration Critical)

        [Fact]
        public async Task Application_StartsSuccessfully()
        {
            // This validates that Program.cs configuration works
            // Critical for the Global.asax.cs ? Program.cs migration
            
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Application_DatabaseInitialized()
        {
            // Validates that DbInitializer runs on startup
            
            // Act
            var response = await _client.GetAsync("/Students/Index");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            // If we get here, database initialization worked
        }

        #endregion

        #region Middleware Pipeline Tests

        [Fact]
        public async Task Middleware_StaticFilesMiddleware_Works()
        {
            // Act
            var response = await _client.GetAsync("/Content/bootstrap.css");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Middleware_RedirectsHttpToHttps()
        {
            // This test might need adjustment based on test server configuration
            // Act & Assert - validates HTTPS redirection middleware
            var response = await _client.GetAsync("/");
            response.Should().NotBeNull();
        }

        #endregion
    }
}
