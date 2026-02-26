using ContosoUniversity.Controllers;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.UnitTests.TestUtilities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace ContosoUniversity.UnitTests.Controllers
{
    /// <summary>
    /// Unit tests for DepartmentsController - validates concurrency handling in EF Core
    /// </summary>
    public class DepartmentsControllerTests : IDisposable
    {
        private readonly SchoolContext _context;
        private readonly FakeNotificationService _notificationService;
        private readonly DepartmentsController _controller;

        public DepartmentsControllerTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _notificationService = new FakeNotificationService();
            _controller = new DepartmentsController(_context, _notificationService);
        }

        #region Index Action Tests

        [Fact]
        public void Index_ReturnsDepartmentsWithAdministrators()
        {
            // Arrange
            var instructor = TestDataBuilder.CreateValidInstructor();
            var department = TestDataBuilder.CreateValidDepartment();
            department.Administrator = instructor;
            
            _context.Instructors.Add(instructor);
            _context.Departments.Add(department);
            _context.SaveChanges();

            // Act
            var result = _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as System.Collections.Generic.List<Department>;
            model.Should().ContainSingle();
            model.First().Administrator.Should().NotBeNull();
        }

        #endregion

        #region Create Action Tests

        [Fact]
        public void Create_ValidDepartment_AddsToDatabase()
        {
            // Arrange
            var instructor = TestDataBuilder.CreateValidInstructor();
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            var department = TestDataBuilder.CreateValidDepartment();
            department.InstructorID = instructor.ID;

            // Act
            var result = _controller.Create(department);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            _context.Departments.Should().ContainSingle();
        }

        [Fact]
        public void Create_SendsNotificationWithDepartmentName()
        {
            // Arrange
            var department = TestDataBuilder.CreateValidDepartment("Computer Science");

            // Act
            _controller.Create(department);

            // Assert
            _notificationService.SentNotifications.Should().ContainSingle();
            var notification = _notificationService.SentNotifications[0];
            notification.EntityType.Should().Be("Department");
            notification.EntityDisplayName.Should().Be("Computer Science");
            notification.Operation.Should().Be(EntityOperation.CREATE);
        }

        #endregion

        #region Edit Action Tests

        [Fact]
        public void Edit_ValidDepartment_UpdatesDatabase()
        {
            // Arrange
            var department = TestDataBuilder.CreateValidDepartment("Original Name");
            _context.Departments.Add(department);
            _context.SaveChanges();
            
            department.Name = "Updated Name";
            department.Budget = 200000;

            // Act
            var result = _controller.Edit(department);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var updated = _context.Departments.First();
            updated.Name.Should().Be("Updated Name");
            updated.Budget.Should().Be(200000);
        }

        #endregion

        #region Concurrency Tests (EF Core Migration Critical)

        [Fact]
        public void Edit_ConcurrentUpdate_DetectsConflict()
        {
            // This test validates that RowVersion concurrency handling works in EF Core
            // Critical for the EF6 ? EF Core migration
            
            // Arrange
            var department = TestDataBuilder.CreateValidDepartment();
            _context.Departments.Add(department);
            _context.SaveChanges();
            
            var originalRowVersion = department.RowVersion;
            
            // Simulate concurrent update by another user
            var departmentCopy = _context.Departments.Find(department.DepartmentID);
            departmentCopy.Name = "Changed by another user";
            _context.SaveChanges();

            // Act - Try to update with stale RowVersion
            department.Name = "My changes";
            _context.Entry(department).State = EntityState.Modified;
            Action act = () => _context.SaveChanges();

            // Assert
            act.Should().Throw<DbUpdateConcurrencyException>();
        }

        [Fact]
        public void Department_RowVersion_UpdatesOnSave()
        {
            // Arrange
            var department = TestDataBuilder.CreateValidDepartment();
            _context.Departments.Add(department);
            _context.SaveChanges();
            var originalRowVersion = department.RowVersion;

            // Act
            department.Name = "Updated";
            _context.SaveChanges();

            // Assert
            department.RowVersion.Should().NotBeEquivalentTo(originalRowVersion);
        }

        #endregion

        #region Delete Action Tests

        [Fact]
        public void DeleteConfirmed_ValidDepartment_RemovesFromDatabase()
        {
            // Arrange
            var department = TestDataBuilder.CreateValidDepartment();
            _context.Departments.Add(department);
            _context.SaveChanges();
            var deptId = department.DepartmentID;

            // Act
            var result = _controller.DeleteConfirmed(deptId);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            _context.Departments.Find(deptId).Should().BeNull();
        }

        [Fact]
        public void DeleteConfirmed_SendsNotification()
        {
            // Arrange
            var department = TestDataBuilder.CreateValidDepartment("Mathematics");
            _context.Departments.Add(department);
            _context.SaveChanges();

            // Act
            _controller.DeleteConfirmed(department.DepartmentID);

            // Assert
            _notificationService.SentNotifications.Should().ContainSingle();
            var notification = _notificationService.SentNotifications[0];
            notification.EntityDisplayName.Should().Be("Mathematics");
            notification.Operation.Should().Be(EntityOperation.DELETE);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void Create_InvalidBudget_ReturnsViewWithError()
        {
            // Arrange
            var department = TestDataBuilder.CreateValidDepartment();
            department.Budget = -1000; // Invalid negative budget
            _controller.ModelState.AddModelError("Budget", "Budget must be positive");

            // Act
            var result = _controller.Create(department);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        #endregion

        public void Dispose()
        {
            _context?.Dispose();
            _controller?.Dispose();
        }
    }
}
