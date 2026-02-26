using ContosoUniversity.Controllers;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.UnitTests.TestUtilities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ContosoUniversity.UnitTests.Controllers
{
    /// <summary>
    /// Unit tests for HomeController - validates basic MVC operations
    /// </summary>
    public class HomeControllerTests : IDisposable
    {
        private readonly SchoolContext _context;
        private readonly FakeNotificationService _notificationService;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _notificationService = new FakeNotificationService();
            _controller = new HomeController(_context, _notificationService);
        }

        [Fact]
        public void Index_ReturnsView()
        {
            // Act
            var result = _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void About_ReturnsViewWithEnrollmentStatistics()
        {
            // Arrange
            var students = new List<Student>
            {
                new Student { FirstMidName = "A", LastName = "Student", EnrollmentDate = System.DateTime.Today.AddYears(-1) },
                new Student { FirstMidName = "B", LastName = "Student", EnrollmentDate = System.DateTime.Today.AddYears(-1) },
                new Student { FirstMidName = "C", LastName = "Student", EnrollmentDate = System.DateTime.Today.AddYears(-2) }
            };
            _context.Students.AddRange(students);
            _context.SaveChanges();

            // Act
            var result = _controller.About();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as System.Collections.Generic.List<ContosoUniversity.Models.SchoolViewModels.EnrollmentDateGroup>;
            model.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public void Contact_ReturnsViewWithMessage()
        {
            // Act
            var result = _controller.Contact();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.ViewData["Message"].Should().Be("Your contact page.");
        }

        [Fact]
        public void Error_ReturnsErrorView()
        {
            // Act
            var result = _controller.Error();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void Unauthorized_ReturnsViewWithMessage()
        {
            // Act
            var result = _controller.Unauthorized();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var message = viewResult.ViewData["Message"] as string;
            message.Should().Contain("permission");
        }

        [Fact]
        public void Controller_InheritsFromBaseController()
        {
            // Validates dependency injection inheritance pattern
            
            // Assert
            _controller.Should().BeAssignableTo<BaseController>();
        }

        public void Dispose()
        {
            _context?.Dispose();
            _controller?.Dispose();
        }
    }
}
