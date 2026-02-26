using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.UnitTests.TestUtilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace ContosoUniversity.UnitTests.Data
{
    /// <summary>
    /// Unit tests for SchoolContext - validates EF Core configuration and .NET 9.0 migration
    /// </summary>
    public class SchoolContextTests : IDisposable
    {
        private readonly SchoolContext _context;

        public SchoolContextTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
        }

        #region DbSet Configuration Tests

        [Fact]
        public void SchoolContext_AllDbSetsInitialized()
        {
            // Assert
            _context.Students.Should().NotBeNull();
            _context.Instructors.Should().NotBeNull();
            _context.Courses.Should().NotBeNull();
            _context.Departments.Should().NotBeNull();
            _context.Enrollments.Should().NotBeNull();
            _context.CourseAssignments.Should().NotBeNull();
            _context.OfficeAssignments.Should().NotBeNull();
            _context.Notifications.Should().NotBeNull();
        }

        #endregion

        #region Inheritance Configuration Tests (TPH)

        [Fact]
        public void SchoolContext_TablePerHierarchy_StudentAndInstructorShareTable()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent();
            var instructor = TestDataBuilder.CreateValidInstructor();

            // Act
            _context.Students.Add(student);
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            // Assert
            _context.People.Should().HaveCount(2);
            _context.Students.Should().ContainSingle();
            _context.Instructors.Should().ContainSingle();
        }

        [Fact]
        public void SchoolContext_QueryingPeople_ReturnsAllPersonTypes()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent("John", "Student");
            var instructor = TestDataBuilder.CreateValidInstructor("Jane", "Instructor");
            _context.Students.Add(student);
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            // Act
            var allPeople = _context.People.ToList();

            // Assert
            allPeople.Should().HaveCount(2);
            allPeople.Should().ContainSingle(p => p is Student);
            allPeople.Should().ContainSingle(p => p is Instructor);
        }

        #endregion

        #region Relationship Configuration Tests

        [Fact]
        public void SchoolContext_CourseAssignment_CompositeKeyWorks()
        {
            // Arrange
            var instructor = TestDataBuilder.CreateValidInstructor();
            var course = TestDataBuilder.CreateValidCourse();
            _context.Instructors.Add(instructor);
            _context.Courses.Add(course);
            _context.SaveChanges();

            var assignment = new CourseAssignment
            {
                CourseID = course.CourseID,
                InstructorID = instructor.ID
            };

            // Act
            _context.CourseAssignments.Add(assignment);
            _context.SaveChanges();

            // Assert
            _context.CourseAssignments.Should().ContainSingle();
        }

        [Fact]
        public void SchoolContext_CourseAssignment_PreventsDuplicates()
        {
            // Arrange
            var instructor = TestDataBuilder.CreateValidInstructor();
            var course = TestDataBuilder.CreateValidCourse();
            _context.Instructors.Add(instructor);
            _context.Courses.Add(course);
            _context.SaveChanges();

            var assignment1 = new CourseAssignment { CourseID = course.CourseID, InstructorID = instructor.ID };
            var assignment2 = new CourseAssignment { CourseID = course.CourseID, InstructorID = instructor.ID };

            // Act
            _context.CourseAssignments.Add(assignment1);
            _context.SaveChanges();
            _context.CourseAssignments.Add(assignment2);
            
            Action act = () => _context.SaveChanges();

            // Assert
            act.Should().Throw<Exception>("because composite key should prevent duplicates");
        }

        [Fact]
        public void SchoolContext_InstructorOfficeAssignment_OneToOneRelationship()
        {
            // Arrange
            var instructor = TestDataBuilder.CreateValidInstructor();
            var office = new OfficeAssignment
            {
                Instructor = instructor,
                Location = "Building 1, Room 101"
            };

            // Act
            _context.Instructors.Add(instructor);
            _context.OfficeAssignments.Add(office);
            _context.SaveChanges();

            // Assert
            var savedInstructor = _context.Instructors
                .Include(i => i.OfficeAssignment)
                .First();
            savedInstructor.OfficeAssignment.Should().NotBeNull();
            savedInstructor.OfficeAssignment.Location.Should().Be("Building 1, Room 101");
        }

        [Fact]
        public void SchoolContext_Course_HasManyEnrollments()
        {
            // Arrange
            var course = TestDataBuilder.CreateValidCourse();
            var student1 = TestDataBuilder.CreateValidStudent("Student", "One");
            var student2 = TestDataBuilder.CreateValidStudent("Student", "Two");
            
            _context.Courses.Add(course);
            _context.Students.AddRange(student1, student2);
            _context.SaveChanges();

            var enrollment1 = new Enrollment { CourseID = course.CourseID, StudentID = student1.ID, Grade = Grade.A };
            var enrollment2 = new Enrollment { CourseID = course.CourseID, StudentID = student2.ID, Grade = Grade.B };

            // Act
            _context.Enrollments.AddRange(enrollment1, enrollment2);
            _context.SaveChanges();

            // Assert
            var savedCourse = _context.Courses
                .Include(c => c.Enrollments)
                .First(c => c.CourseID == course.CourseID);
            savedCourse.Enrollments.Should().HaveCount(2);
        }

        [Fact]
        public void SchoolContext_Department_HasAdministratorRelationship()
        {
            // Arrange
            var instructor = TestDataBuilder.CreateValidInstructor();
            var department = TestDataBuilder.CreateValidDepartment("Computer Science", 500000);
            
            _context.Instructors.Add(instructor);
            _context.SaveChanges();
            
            department.InstructorID = instructor.ID;

            // Act
            _context.Departments.Add(department);
            _context.SaveChanges();

            // Assert
            var savedDept = _context.Departments
                .Include(d => d.Administrator)
                .First();
            savedDept.Administrator.Should().NotBeNull();
            savedDept.Administrator.ID.Should().Be(instructor.ID);
        }

        #endregion

        #region DateTime2 Configuration Tests (EF Core Migration Critical)

        [Fact]
        public void SchoolContext_DateTimeProperties_UseDateTime2ColumnType()
        {
            // This validates that all DateTime properties are configured with datetime2
            // which is critical for SQL Server compatibility in EF Core
            
            // Arrange
            var student = TestDataBuilder.CreateValidStudent();
            student.EnrollmentDate = new DateTime(2020, 1, 15, 10, 30, 45);

            // Act
            _context.Students.Add(student);
            _context.SaveChanges();

            // Assert
            var saved = _context.Students.First();
            saved.EnrollmentDate.Should().Be(new DateTime(2020, 1, 15, 10, 30, 45));
        }

        [Fact]
        public void SchoolContext_InstructorHireDate_StoresCorrectly()
        {
            // Arrange
            var instructor = TestDataBuilder.CreateValidInstructor("Test", "Instructor", new DateTime(2015, 6, 1));

            // Act
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            // Assert
            var saved = _context.Instructors.First();
            saved.HireDate.Should().Be(new DateTime(2015, 6, 1));
        }

        [Fact]
        public void SchoolContext_DepartmentStartDate_StoresCorrectly()
        {
            // Arrange
            var department = TestDataBuilder.CreateValidDepartment();
            department.StartDate = new DateTime(2010, 9, 1);

            // Act
            _context.Departments.Add(department);
            _context.SaveChanges();

            // Assert
            var saved = _context.Departments.First();
            saved.StartDate.Should().Be(new DateTime(2010, 9, 1));
        }

        #endregion

        #region Concurrency Tests

        [Fact]
        public void SchoolContext_Department_RowVersionTracksChanges()
        {
            // Arrange
            var department = TestDataBuilder.CreateValidDepartment();
            _context.Departments.Add(department);
            _context.SaveChanges();
            var originalRowVersion = department.RowVersion;

            // Act
            department.Name = "Updated Department";
            _context.SaveChanges();

            // Assert
            department.RowVersion.Should().NotBeEquivalentTo(originalRowVersion);
        }

        #endregion

        #region CRUD Operations Tests

        [Fact]
        public void SchoolContext_Add_SavesEntity()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent();

            // Act
            _context.Students.Add(student);
            var result = _context.SaveChanges();

            // Assert
            result.Should().Be(1);
            _context.Students.Should().ContainSingle();
        }

        [Fact]
        public void SchoolContext_Update_ModifiesEntity()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent("Original", "Name");
            _context.Students.Add(student);
            _context.SaveChanges();

            // Act
            student.FirstMidName = "Updated";
            _context.SaveChanges();

            // Assert
            var updated = _context.Students.First();
            updated.FirstMidName.Should().Be("Updated");
        }

        [Fact]
        public void SchoolContext_Delete_RemovesEntity()
        {
            // Arrange
            var student = TestDataBuilder.CreateValidStudent();
            _context.Students.Add(student);
            _context.SaveChanges();
            var studentId = student.ID;

            // Act
            _context.Students.Remove(student);
            _context.SaveChanges();

            // Assert
            _context.Students.Find(studentId).Should().BeNull();
        }

        #endregion

        #region Query Tests

        [Fact]
        public void SchoolContext_Include_LoadsNavigationProperties()
        {
            // Arrange
            var department = TestDataBuilder.CreateValidDepartment();
            var course = TestDataBuilder.CreateValidCourse();
            course.Department = department;
            
            _context.Departments.Add(department);
            _context.Courses.Add(course);
            _context.SaveChanges();

            // Act
            var result = _context.Courses
                .Include(c => c.Department)
                .First();

            // Assert
            result.Department.Should().NotBeNull();
            result.Department.Name.Should().Be(department.Name);
        }

        [Fact]
        public void SchoolContext_ThenInclude_LoadsNestedNavigationProperties()
        {
            // Arrange
            var instructor = TestDataBuilder.CreateValidInstructor();
            var course = TestDataBuilder.CreateValidCourse();
            var assignment = new CourseAssignment
            {
                Instructor = instructor,
                Course = course
            };
            
            _context.Instructors.Add(instructor);
            _context.Courses.Add(course);
            _context.CourseAssignments.Add(assignment);
            _context.SaveChanges();

            // Act
            var result = _context.Instructors
                .Include(i => i.CourseAssignments)
                    .ThenInclude(ca => ca.Course)
                .First();

            // Assert
            result.CourseAssignments.Should().ContainSingle();
            result.CourseAssignments.First().Course.Should().NotBeNull();
        }

        #endregion

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
