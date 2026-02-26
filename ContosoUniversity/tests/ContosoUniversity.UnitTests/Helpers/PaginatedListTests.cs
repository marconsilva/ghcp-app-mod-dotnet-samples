using ContosoUniversity.Models;
using ContosoUniversity.UnitTests.TestUtilities;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace ContosoUniversity.UnitTests.Helpers
{
    /// <summary>
    /// Unit tests for PaginatedList - validates pagination logic
    /// </summary>
    public class PaginatedListTests
    {
        [Fact]
        public void Create_WithLessThanPageSize_ReturnsSinglePage()
        {
            // Arrange
            var students = TestDataBuilder.CreateStudents(5).AsQueryable();

            // Act
            var result = PaginatedList<Student>.Create(students, 1, 10);

            // Assert
            result.Should().HaveCount(5);
            result.PageIndex.Should().Be(1);
            result.TotalPages.Should().Be(1);
            result.HasPreviousPage.Should().BeFalse();
            result.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public void Create_WithMultiplePages_CalculatesCorrectTotalPages()
        {
            // Arrange
            var students = TestDataBuilder.CreateStudents(25).AsQueryable();

            // Act
            var result = PaginatedList<Student>.Create(students, 1, 10);

            // Assert
            result.TotalPages.Should().Be(3);
        }

        [Fact]
        public void Create_FirstPage_HasNoPreviousPage()
        {
            // Arrange
            var students = TestDataBuilder.CreateStudents(25).AsQueryable();

            // Act
            var result = PaginatedList<Student>.Create(students, 1, 10);

            // Assert
            result.HasPreviousPage.Should().BeFalse();
            result.HasNextPage.Should().BeTrue();
        }

        [Fact]
        public void Create_LastPage_HasNoNextPage()
        {
            // Arrange
            var students = TestDataBuilder.CreateStudents(25).AsQueryable();

            // Act
            var result = PaginatedList<Student>.Create(students, 3, 10);

            // Assert
            result.HasPreviousPage.Should().BeTrue();
            result.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public void Create_MiddlePage_HasBothNavigationOptions()
        {
            // Arrange
            var students = TestDataBuilder.CreateStudents(30).AsQueryable();

            // Act
            var result = PaginatedList<Student>.Create(students, 2, 10);

            // Assert
            result.HasPreviousPage.Should().BeTrue();
            result.HasNextPage.Should().BeTrue();
        }

        [Fact]
        public void Create_EmptyCollection_ReturnsEmptyList()
        {
            // Arrange
            var students = new Student[0].AsQueryable();

            // Act
            var result = PaginatedList<Student>.Create(students, 1, 10);

            // Assert
            result.Should().BeEmpty();
            result.TotalPages.Should().Be(0);
        }

        [Fact]
        public void Create_PageIndexZero_TreatsAsPageOne()
        {
            // Arrange
            var students = TestDataBuilder.CreateStudents(15).AsQueryable();

            // Act
            var result = PaginatedList<Student>.Create(students, 0, 10);

            // Assert
            result.PageIndex.Should().Be(0);
            result.Should().HaveCount(10);
        }

        [Fact]
        public void Create_PageSizeOne_CreatesMultiplePages()
        {
            // Arrange
            var students = TestDataBuilder.CreateStudents(5).AsQueryable();

            // Act
            var result = PaginatedList<Student>.Create(students, 1, 1);

            // Assert
            result.Should().HaveCount(1);
            result.TotalPages.Should().Be(5);
        }

        [Fact]
        public void Create_RequestPageBeyondTotalPages_ReturnsLastPage()
        {
            // Arrange
            var students = TestDataBuilder.CreateStudents(15).AsQueryable();

            // Act
            var result = PaginatedList<Student>.Create(students, 10, 10);

            // Assert
            result.PageIndex.Should().Be(10);
            result.Should().BeEmpty();
        }
    }
}
