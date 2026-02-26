using ContosoUniversity.Controllers;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.UnitTests.TestUtilities;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace ContosoUniversity.UnitTests.Controllers
{
    /// <summary>
    /// Unit tests for CoursesController - validates CRUD operations and file upload modernization (IFormFile)
    /// </summary>
    public class CoursesControllerTests : IDisposable
    {
        private readonly SchoolContext _context;
        private readonly FakeNotificationService _notificationService;
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;
        private readonly CoursesController _controller;
        private readonly string _testUploadPath;

        public CoursesControllerTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _notificationService = new FakeNotificationService();
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            
            // Set up temporary test upload path
            _testUploadPath = Path.Combine(Path.GetTempPath(), "ContosoUniversityTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testUploadPath);
            
            _mockEnvironment.Setup(e => e.WebRootPath).Returns(_testUploadPath);
            
            _controller = new CoursesController(_context, _notificationService, _mockEnvironment.Object);
            
            // Seed a test department
            _context.Departments.Add(TestDataBuilder.CreateValidDepartment());
            _context.SaveChanges();
        }

        #region Index Action Tests

        [Fact]
        public void Index_ReturnsViewWithCourses()
        {
            // Arrange
            var courses = TestDataBuilder.CreateCourses(5);
            _context.Courses.AddRange(courses);
            _context.SaveChanges();

            // Act
            var result = _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as System.Collections.Generic.List<Course>;
            model.Should().HaveCount(5);
        }

        #endregion

        #region Create with File Upload Tests (Critical Modernization Test)

        [Fact]
        public void Create_WithValidImageFile_SavesFileToWwwroot()
        {
            // Arrange
            var course = TestDataBuilder.CreateValidCourse();
            var mockFile = CreateMockFormFile("test.jpg", "image/jpeg", 1024);

            // Act
            var result = _controller.Create(course, mockFile);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            course.TeachingMaterialImagePath.Should().NotBeNullOrEmpty();
            course.TeachingMaterialImagePath.Should().StartWith("~/Uploads/TeachingMaterials/");
        }

        [Fact]
        public void Create_UsesIWebHostEnvironment_ForPathResolution()
        {
            // This validates that the modernized code uses IWebHostEnvironment instead of Server.MapPath
            
            // Arrange
            var course = TestDataBuilder.CreateValidCourse();
            var mockFile = CreateMockFormFile("test.jpg", "image/jpeg", 1024);

            // Act
            _controller.Create(course, mockFile);

            // Assert
            _mockEnvironment.Verify(e => e.WebRootPath, Times.AtLeastOnce());
        }

        [Fact]
        public void Create_InvalidFileType_ReturnsViewWithError()
        {
            // Arrange
            var course = TestDataBuilder.CreateValidCourse();
            var mockFile = CreateMockFormFile("test.txt", "text/plain", 1024);

            // Act
            var result = _controller.Create(course, mockFile);

            // Assert
            result.Should().BeOfType<ViewResult>();
            _controller.ModelState.IsValid.Should().BeFalse();
            _controller.ModelState.ContainsKey("teachingMaterialImage").Should().BeTrue();
        }

        [Fact]
        public void Create_FileTooLarge_ReturnsViewWithError()
        {
            // Arrange
            var course = TestDataBuilder.CreateValidCourse();
            var mockFile = CreateMockFormFile("test.jpg", "image/jpeg", 6 * 1024 * 1024); // 6MB

            // Act
            var result = _controller.Create(course, mockFile);

            // Assert
            result.Should().BeOfType<ViewResult>();
            _controller.ModelState.IsValid.Should().BeFalse();
            _controller.ModelState["teachingMaterialImage"].Errors[0].ErrorMessage
                .Should().Contain("5MB");
        }

        [Fact]
        public void Create_ValidFileExtensions_AreAccepted()
        {
            // Test all valid file extensions
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

            foreach (var extension in validExtensions)
            {
                // Arrange
                _context.ChangeTracker.Clear();
                var course = TestDataBuilder.CreateValidCourse(courseId: 1000 + validExtensions.ToList().IndexOf(extension));
                var mockFile = CreateMockFormFile($"test{extension}", "image/jpeg", 1024);

                // Act
                var result = _controller.Create(course, mockFile);

                // Assert
                result.Should().BeOfType<RedirectToActionResult>($"because {extension} should be valid");
            }
        }

        [Fact]
        public void Create_GeneratesUniqueFileName()
        {
            // Arrange
            var course1 = TestDataBuilder.CreateValidCourse(courseId: 2001);
            var course2 = TestDataBuilder.CreateValidCourse(courseId: 2002);
            var mockFile1 = CreateMockFormFile("test.jpg", "image/jpeg", 1024);
            var mockFile2 = CreateMockFormFile("test.jpg", "image/jpeg", 1024);

            // Act
            _controller.Create(course1, mockFile1);
            _controller.Create(course2, mockFile2);

            // Assert
            course1.TeachingMaterialImagePath.Should().NotBe(course2.TeachingMaterialImagePath);
        }

        [Fact]
        public void Create_WithoutFile_WorksCorrectly()
        {
            // Arrange
            var course = TestDataBuilder.CreateValidCourse();

            // Act
            var result = _controller.Create(course, null);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            _context.Courses.Should().ContainSingle();
            course.TeachingMaterialImagePath.Should().BeNullOrEmpty();
        }

        #endregion

        #region Edit with File Upload Tests

        [Fact]
        public void Edit_WithNewFile_DeletesOldFile()
        {
            // Arrange
            var course = TestDataBuilder.CreateValidCourse();
            course.TeachingMaterialImagePath = "~/Uploads/TeachingMaterials/oldfile.jpg";
            _context.Courses.Add(course);
            _context.SaveChanges();
            
            // Create the old file
            var oldFilePath = Path.Combine(_testUploadPath, "Uploads", "TeachingMaterials", "oldfile.jpg");
            Directory.CreateDirectory(Path.GetDirectoryName(oldFilePath));
            File.WriteAllText(oldFilePath, "old content");
            
            var newMockFile = CreateMockFormFile("newfile.jpg", "image/jpeg", 1024);

            // Act
            _controller.Edit(course, newMockFile);

            // Assert
            File.Exists(oldFilePath).Should().BeFalse("old file should be deleted");
            course.TeachingMaterialImagePath.Should().NotContain("oldfile.jpg");
        }

        #endregion

        #region Notification Integration Tests

        [Fact]
        public void Create_SendsCorrectNotificationData()
        {
            // Arrange
            var course = TestDataBuilder.CreateValidCourse(title: "Advanced Mathematics");

            // Act
            _controller.Create(course, null);

            // Assert
            var notification = _notificationService.SentNotifications.Should().ContainSingle().Subject;
            notification.EntityId.Should().Be(course.CourseID.ToString());
            notification.EntityDisplayName.Should().Be("Advanced Mathematics");
            notification.Operation.Should().Be(EntityOperation.CREATE);
        }

        #endregion

        #region ViewBag and SelectList Tests

        [Fact]
        public void Create_Get_PopulatesDepartmentSelectList()
        {
            // Arrange
            var department = TestDataBuilder.CreateValidDepartment("Test Dept");
            _context.Departments.Add(department);
            _context.SaveChanges();

            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = result as ViewResult;
            viewResult.ViewData["DepartmentID"].Should().BeOfType<SelectList>();
            var selectList = viewResult.ViewData["DepartmentID"] as SelectList;
            selectList.Should().NotBeEmpty();
        }

        #endregion

        #region Helper Methods

        private IFormFile CreateMockFormFile(string fileName, string contentType, long length)
        {
            var fileMock = new Mock<IFormFile>();
            var content = new byte[length];
            var ms = new MemoryStream(content);
            
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.ContentType).Returns(contentType);
            fileMock.Setup(f => f.Length).Returns(length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.CopyTo(It.IsAny<Stream>()))
                .Callback<Stream>(stream => ms.CopyTo(stream));
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                .Returns<Stream, System.Threading.CancellationToken>((stream, token) => 
                {
                    ms.CopyTo(stream);
                    return System.Threading.Tasks.Task.CompletedTask;
                });

            return fileMock.Object;
        }

        #endregion

        public void Dispose()
        {
            _context?.Dispose();
            _controller?.Dispose();
            
            // Clean up test files
            if (Directory.Exists(_testUploadPath))
            {
                try
                {
                    Directory.Delete(_testUploadPath, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
    }
}
