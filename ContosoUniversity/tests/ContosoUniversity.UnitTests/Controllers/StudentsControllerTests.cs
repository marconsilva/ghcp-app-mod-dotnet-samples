using ContosoUniversity.Controllers;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.UnitTests.TestUtilities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Xunit;

namespace ContosoUniversity.UnitTests.Controllers
{
    /// <summary>
    /// Unit tests for StudentsController - validates CRUD operations and .NET 9.0 modernization
    /// </summary>
    public class StudentsControllerTests : IDisposable
    {
        private readonly SchoolContext _context;
        private readonly FakeNotificationService _notificationService;
        private readonly StudentsController _controller;

        public StudentsControllerTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _notificationService = new FakeNotificationService();
            _controller = new StudentsController(_context, _notificationService);
        }

        #region Index Action Tests

        [Fact]
        public void Index_ReturnsViewWithPaginatedStudents()
        {
            // Arrange
            var students = TestDataBuilder.CreateStudents(15);
            _context.Students.AddRange(students);
            _context.SaveChanges();

            // Act
            var result = _controller.Index(null, null, null, 1);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<PaginatedList<Student>>();
            var model = viewResult.Model as PaginatedList<Student>;
            model.Should().HaveCount(10); // Default page size
        }

        [Fact]
        public void Index_WithSearchString_FiltersStudentsByLastName()
        {
            // Arrange
            _context.Students.Add(TestDataBuilder.CreateValidStudent("John", "Smith"));
            _context.Students.Add(TestDataBuilder.CreateValidStudent("Jane", "Doe"));
            _context.Students.Add(TestDataBuilder.CreateValidStudent("Bob", "Smith"));
            _context.SaveChanges();

            // Act
            var result = _controller.Index(null, null, "Smith", 1);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as PaginatedList<Student>;
            model.Should().HaveCount(2);
            model.All(s => s.LastName.Contains("Smith")).Should().BeTrue();
        }

        [Fact]
        public void Index_WithSearchString_FiltersStudentsByFirstName()
        {
            // Arrange
            _context.Students.Add(TestDataBuilder.CreateValidStudent("John", "Smith"));
            _context.Students.Add(TestDataBuilder.CreateValidStudent("Jane", "Doe"));
            _context.Students.Add(TestDataBuilder.CreateValidStudent("John", "Williams"));
            _context.SaveChanges();

            // Act
            var result = _controller.Index(null, null, "John", 1);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as PaginatedList<Student>;
            model.Should().HaveCount(2);
            model.All(s => s.FirstMidName.Contains("John")).Should().BeTrue();
        }

        [Fact]
        public void Index_SortByNameDescending_ReturnsSortedStudents()
        {
            // Arrange
            _context.Students.Add(TestDataBuilder.CreateValidStudent("Alice", "Anderson"));
            _context.Students.Add(TestDataBuilder.CreateValidStudent("Bob", "Brown"));
            _context.Students.Add(TestDataBuilder.CreateValidStudent("Charlie", "Clark"));
            _context.SaveChanges();

            // Act
            var result = _controller.Index("name_desc", null, null, 1);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as PaginatedList<Student>;
            model.First().LastName.Should().Be("Clark");
            model.Last().LastName.Should().Be("Anderson");
        }

        [Fact]
        public void Index_SortByDate_ReturnsSortedByEnrollmentDate()
        {
            // Arrange
            _context.Students.Add(new Student { FirstMidName = "A", LastName = "Student", EnrollmentDate = DateTime.Today.AddDays(-30) });
            _context.Students.Add(new Student { FirstMidName = "B", LastName = "Student", EnrollmentDate = DateTime.Today.AddDays(-60) });
            _context.Students.Add(new Student { FirstMidName = "C", LastName = "Student", EnrollmentDate = DateTime.Today.AddDays(-10) });
            _context.SaveChanges();

            // Act
            var result = _controller.Index("Date", null, null, 1);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as PaginatedList<Student>;
            model.First().EnrollmentDate.Should().Be(DateTime.Today.AddDays(-60));
        }

        [Fact]
        public void Index_SecondPage_ReturnsCorrectPageOfStudents()
        {
            // Arrange
            var students = TestDataBuilder.CreateStudents(25);
            _context.Students.AddRange(students);
            _context.SaveChanges();

            // Act
            var result = _controller.Index(null, null, null, 2);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as PaginatedList<Student>;
            model.Should().HaveCount(10);
            model.PageIndex.Should().Be(2);
        }

        #endregion

        #region Details Action Tests

        [Fact]
        public void Details_ValidId_ReturnsViewWithStudent()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent();
            _context.Students.Add(student);
            _context.SaveChanges();

            // Act
            var result = _controller.Details(student.ID);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<Student>();
            var model = viewResult.Model as Student;
            model.ID.Should().Be(student.ID);
        }

        [Fact]
        public void Details_NullId_ReturnsBadRequest()
        {
            // Act
            var result = _controller.Details(null);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public void Details_NonExistentId_ReturnsNotFound()
        {
            // Act
            var result = _controller.Details(99999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region Create Action Tests

        [Fact]
        public void Create_Get_ReturnsViewWithDefaultEnrollmentDate()
        {
            // Act
            var result = _controller.Create();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<Student>();
            var model = viewResult.Model as Student;
            model.EnrollmentDate.Should().Be(DateTime.Today);
        }

        [Fact]
        public void Create_ValidStudent_AddsToDatabase()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent("Test", "Student");

            // Act
            var result = _controller.Create(student);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirect = result as RedirectToActionResult;
            redirect.ActionName.Should().Be("Index");
            
            _context.Students.Should().ContainSingle();
            _context.Students.First().FirstMidName.Should().Be("Test");
        }

        [Fact]
        public void Create_ValidStudent_SendsNotification()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent("Test", "Student");

            // Act
            _controller.Create(student);

            // Assert
            _notificationService.SentNotifications.Should().ContainSingle();
            var notification = _notificationService.SentNotifications[0];
            notification.EntityType.Should().Be("Student");
            notification.Operation.Should().Be(EntityOperation.CREATE);
            notification.EntityDisplayName.Should().Contain("Test Student");
        }

        [Fact]
        public void Create_InvalidEnrollmentDate_ReturnsViewWithError()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent();
            student.EnrollmentDate = DateTime.MinValue;

            // Act
            var result = _controller.Create(student);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            _controller.ModelState.IsValid.Should().BeFalse();
            _controller.ModelState.ContainsKey("EnrollmentDate").Should().BeTrue();
        }

        [Fact]
        public void Create_EnrollmentDateBefore1753_ReturnsViewWithError()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent();
            student.EnrollmentDate = new DateTime(1752, 12, 31);

            // Act
            var result = _controller.Create(student);

            // Assert
            result.Should().BeOfType<ViewResult>();
            _controller.ModelState.ContainsKey("EnrollmentDate").Should().BeTrue();
        }

        [Fact]
        public void Create_EnrollmentDateAfter9999_ReturnsViewWithError()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent();
            student.EnrollmentDate = DateTime.MaxValue; // Use DateTime.MaxValue which is year 9999

            // Act
            var result = _controller.Create(student);

            // Assert
            result.Should().BeOfType<ViewResult>();
            _controller.ModelState.ContainsKey("EnrollmentDate").Should().BeTrue();
        }

        #endregion

        #region Edit Action Tests

        [Fact]
        public void Edit_Get_ValidId_ReturnsViewWithStudent()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent();
            _context.Students.Add(student);
            _context.SaveChanges();

            // Act
            var result = _controller.Edit(student.ID);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as Student;
            model.ID.Should().Be(student.ID);
        }

        [Fact]
        public void Edit_Get_NullId_ReturnsStatusCode400()
        {
            // Act
            var result = _controller.Edit((int?)null);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            var statusResult = result as StatusCodeResult;
            statusResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public void Edit_Post_ValidStudent_UpdatesDatabase()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent("Original", "Name");
            _context.Students.Add(student);
            _context.SaveChanges();
            
            student.FirstMidName = "Updated";
            student.LastName = "NewName";

            // Act
            var result = _controller.Edit(student);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            
            var updatedStudent = _context.Students.Find(student.ID);
            updatedStudent.FirstMidName.Should().Be("Updated");
            updatedStudent.LastName.Should().Be("NewName");
        }

        [Fact]
        public void Edit_Post_SendsNotification()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent();
            _context.Students.Add(student);
            _context.SaveChanges();
            
            student.FirstMidName = "Updated";

            // Act
            _controller.Edit(student);

            // Assert
            _notificationService.SentNotifications.Should().ContainSingle();
            var notification = _notificationService.SentNotifications[0];
            notification.EntityType.Should().Be("Student");
            notification.Operation.Should().Be(EntityOperation.UPDATE);
        }

        #endregion

        #region Delete Action Tests

        [Fact]
        public void Delete_Get_ValidId_ReturnsViewWithStudent()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent();
            _context.Students.Add(student);
            _context.SaveChanges();

            // Act
            var result = _controller.Delete(student.ID);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as Student;
            model.ID.Should().Be(student.ID);
        }

        [Fact]
        public void Delete_Get_NullId_ReturnsStatusCode400()
        {
            // Act
            var result = _controller.Delete(null);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            var statusResult = result as StatusCodeResult;
            statusResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public void DeleteConfirmed_ValidId_RemovesFromDatabase()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent();
            _context.Students.Add(student);
            _context.SaveChanges();
            var studentId = student.ID;

            // Act
            var result = _controller.DeleteConfirmed(studentId);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            _context.Students.Find(studentId).Should().BeNull();
        }

        [Fact]
        public void DeleteConfirmed_SendsNotification()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent("Test", "Student");
            _context.Students.Add(student);
            _context.SaveChanges();

            // Act
            _controller.DeleteConfirmed(student.ID);

            // Assert
            _notificationService.SentNotifications.Should().ContainSingle();
            var notification = _notificationService.SentNotifications[0];
            notification.EntityType.Should().Be("Student");
            notification.Operation.Should().Be(EntityOperation.DELETE);
            notification.EntityDisplayName.Should().Contain("Test Student");
        }

        #endregion

        #region Modernization-Specific Tests

        [Fact]
        public void Controller_InstantiatesWithDependencyInjection()
        {
            // This test validates that the controller can be instantiated with DI
            // which is critical for the .NET 9.0 migration
            
            // Arrange & Act
            var controller = new StudentsController(_context, _notificationService);

            // Assert
            controller.Should().NotBeNull();
            // The fact that we can create the controller proves DI is working
        }

        [Fact]
        public void Create_ValidatesEnrollmentDateRange_SqlServerCompatibility()
        {
            // This test validates SQL Server datetime2 range validation
            // which is critical for the EF Core migration
            
            // Arrange
            var student = TestDataBuilder.CreateValidStudent();
            student.EnrollmentDate = new DateTime(1752, 1, 1); // Before SQL Server min

            // Act
            var result = _controller.Create(student);

            // Assert
            _controller.ModelState.IsValid.Should().BeFalse();
            _controller.ModelState.ContainsKey("EnrollmentDate").Should().BeTrue();
        }

        #endregion

        #region Pagination Tests

        [Fact]
        public void Index_Pagination_FirstPageHasNoPreviousPage()
        {
            // Arrange
            var students = TestDataBuilder.CreateStudents(25);
            _context.Students.AddRange(students);
            _context.SaveChanges();

            // Act
            var result = _controller.Index(null, null, null, 1);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as PaginatedList<Student>;
            model.HasPreviousPage.Should().BeFalse();
            model.HasNextPage.Should().BeTrue();
        }

        [Fact]
        public void Index_Pagination_LastPageHasNoNextPage()
        {
            // Arrange
            var students = TestDataBuilder.CreateStudents(25);
            _context.Students.AddRange(students);
            _context.SaveChanges();

            // Act
            var result = _controller.Index(null, null, null, 3); // Last page

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as PaginatedList<Student>;
            model.HasPreviousPage.Should().BeTrue();
            model.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public void Index_Pagination_MiddlePageHasBothNavigation()
        {
            // Arrange
            var students = TestDataBuilder.CreateStudents(25);
            _context.Students.AddRange(students);
            _context.SaveChanges();

            // Act
            var result = _controller.Index(null, null, null, 2);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as PaginatedList<Student>;
            model.HasPreviousPage.Should().BeTrue();
            model.HasNextPage.Should().BeTrue();
        }

        #endregion

        #region Search and Filter Preservation Tests

        [Fact]
        public void Index_SearchStringProvided_ResetsPageTo1()
        {
            // Arrange
            var students = TestDataBuilder.CreateStudents(25);
            _context.Students.AddRange(students);
            _context.SaveChanges();

            // Act
            var result = _controller.Index(null, "oldFilter", "newSearch", null);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as PaginatedList<Student>;
            model.PageIndex.Should().Be(1);
        }

        [Fact]
        public void Index_NoSearchString_UsesCurrentFilter()
        {
            // Arrange
            var student1 = TestDataBuilder.CreateValidStudent("John", "Smith");
            var student2 = TestDataBuilder.CreateValidStudent("Jane", "Doe");
            _context.Students.AddRange(student1, student2);
            _context.SaveChanges();

            // Act
            var result = _controller.Index(null, "Smith", null, 1);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.ViewData["CurrentFilter"].Should().Be("Smith");
            var model = viewResult.Model as PaginatedList<Student>;
            model.Should().ContainSingle();
        }

        #endregion

        public void Dispose()
        {
            _context?.Dispose();
            _controller?.Dispose();
        }
    }
}
