using ContosoUniversity.Models;
using Bogus;
using System;
using System.Collections.Generic;

namespace ContosoUniversity.UnitTests.TestUtilities
{
    /// <summary>
    /// Builder class for creating test data using Bogus library
    /// </summary>
    public static class TestDataBuilder
    {
        public static Faker<Student> StudentFaker => new Faker<Student>()
            .RuleFor(s => s.FirstMidName, f => f.Name.FirstName())
            .RuleFor(s => s.LastName, f => f.Name.LastName())
            .RuleFor(s => s.EnrollmentDate, f => f.Date.Between(DateTime.Now.AddYears(-4), DateTime.Now))
            .RuleFor(s => s.Enrollments, f => new List<Enrollment>());

        public static Faker<Instructor> InstructorFaker => new Faker<Instructor>()
            .RuleFor(i => i.FirstMidName, f => f.Name.FirstName())
            .RuleFor(i => i.LastName, f => f.Name.LastName())
            .RuleFor(i => i.HireDate, f => f.Date.Between(DateTime.Now.AddYears(-20), DateTime.Now))
            .RuleFor(i => i.CourseAssignments, f => new List<CourseAssignment>());

        public static Faker<Course> CourseFaker => new Faker<Course>()
            .RuleFor(c => c.CourseID, f => f.Random.Int(1000, 9999))
            .RuleFor(c => c.Title, f => f.Lorem.Sentence(3))
            .RuleFor(c => c.Credits, f => f.Random.Int(1, 5))
            .RuleFor(c => c.DepartmentID, f => 1)
            .RuleFor(c => c.Enrollments, f => new List<Enrollment>())
            .RuleFor(c => c.CourseAssignments, f => new List<CourseAssignment>());

        public static Faker<Department> DepartmentFaker => new Faker<Department>()
            .RuleFor(d => d.Name, f => f.Commerce.Department())
            .RuleFor(d => d.Budget, f => f.Finance.Amount(10000, 1000000))
            .RuleFor(d => d.StartDate, f => f.Date.Between(DateTime.Now.AddYears(-10), DateTime.Now))
            .RuleFor(d => d.Courses, f => new List<Course>());

        public static Faker<Enrollment> EnrollmentFaker => new Faker<Enrollment>()
            .RuleFor(e => e.Grade, f => f.PickRandom<Grade?>());

        public static Faker<OfficeAssignment> OfficeAssignmentFaker => new Faker<OfficeAssignment>()
            .RuleFor(o => o.Location, f => f.Address.BuildingNumber() + " " + f.Address.StreetName());

        /// <summary>
        /// Creates a student with valid test data
        /// </summary>
        public static Student CreateValidStudent(string firstName = "John", string lastName = "Doe")
        {
            return new Student
            {
                FirstMidName = firstName,
                LastName = lastName,
                EnrollmentDate = DateTime.Today,
                Enrollments = new List<Enrollment>()
            };
        }

        /// <summary>
        /// Creates a course with valid test data
        /// </summary>
        public static Course CreateValidCourse(int courseId = 1050, string title = "Test Course", int credits = 3, int departmentId = 1)
        {
            return new Course
            {
                CourseID = courseId,
                Title = title,
                Credits = credits,
                DepartmentID = departmentId,
                Enrollments = new List<Enrollment>(),
                CourseAssignments = new List<CourseAssignment>()
            };
        }

        /// <summary>
        /// Creates an instructor with valid test data
        /// </summary>
        public static Instructor CreateValidInstructor(string firstName = "Jane", string lastName = "Smith", DateTime? hireDate = null)
        {
            return new Instructor
            {
                FirstMidName = firstName,
                LastName = lastName,
                HireDate = hireDate ?? DateTime.Today.AddYears(-5),
                CourseAssignments = new List<CourseAssignment>()
            };
        }

        /// <summary>
        /// Creates a department with valid test data
        /// </summary>
        public static Department CreateValidDepartment(string name = "Test Department", decimal budget = 100000, int? instructorId = null)
        {
            return new Department
            {
                Name = name,
                Budget = budget,
                StartDate = DateTime.Today.AddYears(-5),
                InstructorID = instructorId,
                Courses = new List<Course>(),
                RowVersion = new byte[8]
            };
        }

        /// <summary>
        /// Creates multiple students using Bogus
        /// </summary>
        public static List<Student> CreateStudents(int count)
        {
            return StudentFaker.Generate(count);
        }

        /// <summary>
        /// Creates multiple courses using Bogus
        /// </summary>
        public static List<Course> CreateCourses(int count)
        {
            return CourseFaker.Generate(count);
        }

        /// <summary>
        /// Creates multiple instructors using Bogus
        /// </summary>
        public static List<Instructor> CreateInstructors(int count)
        {
            return InstructorFaker.Generate(count);
        }

        /// <summary>
        /// Creates multiple departments using Bogus
        /// </summary>
        public static List<Department> CreateDepartments(int count)
        {
            return DepartmentFaker.Generate(count);
        }
    }
}
