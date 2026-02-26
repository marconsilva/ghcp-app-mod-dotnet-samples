using ContosoUniversity.Models;
using FluentAssertions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Xunit;

namespace ContosoUniversity.UnitTests.Models
{
    /// <summary>
    /// Unit tests for Student model - validates data annotations and business rules
    /// </summary>
    public class StudentTests
    {
        [Fact]
        public void Student_InheritsFromPerson()
        {
            // Arrange & Act
            var student = new Student();

            // Assert
            student.Should().BeAssignableTo<Person>();
        }

        [Fact]
        public void Student_EnrollmentDateRequired_ValidationWorks()
        {
            // Arrange
            var student = new Student
            {
                FirstMidName = "John",
                LastName = "Doe"
                // EnrollmentDate not set
            };

            // Act
            var validationResults = ValidateModel(student);

            // Assert
            validationResults.Should().Contain(v => v.MemberNames.Contains("EnrollmentDate"));
        }

        [Fact]
        public void Student_ValidEnrollmentDate_PassesValidation()
        {
            // Arrange
            var student = new Student
            {
                FirstMidName = "John",
                LastName = "Doe",
                EnrollmentDate = DateTime.Today
            };

            // Act
            var validationResults = ValidateModel(student);

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void Student_EnrollmentDateBefore1753_FailsValidation()
        {
            // Arrange
            var student = new Student
            {
                FirstMidName = "John",
                LastName = "Doe",
                EnrollmentDate = new DateTime(1752, 12, 31)
            };

            // Act
            var validationResults = ValidateModel(student);

            // Assert
            validationResults.Should().Contain(v => 
                v.MemberNames.Contains("EnrollmentDate") && 
                v.ErrorMessage.Contains("1753"));
        }

        [Fact]
        public void Student_EnrollmentDateAfter9999_FailsValidation()
        {
            // Arrange
            var student = new Student
            {
                FirstMidName = "John",
                LastName = "Doe",
                EnrollmentDate = new DateTime(10000, 1, 1)
            };

            // Act
            var validationResults = ValidateModel(student);

            // Assert
            validationResults.Should().Contain(v => 
                v.MemberNames.Contains("EnrollmentDate") && 
                v.ErrorMessage.Contains("9999"));
        }

        [Fact]
        public void Student_EnrollmentsCollection_InitializesAsEmpty()
        {
            // Arrange & Act
            var student = new Student();

            // Assert
            student.Enrollments.Should().BeNull();
        }

        [Fact]
        public void Student_CanHaveMultipleEnrollments()
        {
            // Arrange
            var student = new Student
            {
                FirstMidName = "John",
                LastName = "Doe",
                EnrollmentDate = DateTime.Today,
                Enrollments = new List<Enrollment>
                {
                    new Enrollment { CourseID = 1050, Grade = Grade.A },
                    new Enrollment { CourseID = 1051, Grade = Grade.B }
                }
            };

            // Act & Assert
            student.Enrollments.Should().HaveCount(2);
        }

        #region Helper Methods

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        #endregion
    }
}
