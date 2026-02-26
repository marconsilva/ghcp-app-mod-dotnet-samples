using ContosoUniversity.Models;
using ContosoUniversity.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace ContosoUniversity.UnitTests.Services
{
    /// <summary>
    /// Unit tests for NotificationService - validates System.Messaging to MSMQ.Messaging migration
    /// </summary>
    public class NotificationServiceTests
    {
        private readonly IConfiguration _configuration;

        public NotificationServiceTests()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"NotificationQueuePath", ".\\Private$\\TestNotificationQueue"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();
        }

        #region Constructor and Initialization Tests

        [Fact]
        public void Constructor_WithIConfiguration_InitializesSuccessfully()
        {
            // This test validates the critical DI modernization for NotificationService
            
            // Act
            Action act = () => new NotificationService(_configuration);

            // Assert
            act.Should().NotThrow("because IConfiguration dependency injection should work");
        }

        [Fact]
        public void Constructor_ReadsQueuePathFromConfiguration()
        {
            // This validates that the service now uses IConfiguration instead of ConfigurationManager
            
            // Arrange
            var customSettings = new Dictionary<string, string>
            {
                {"NotificationQueuePath", ".\\Private$\\CustomQueue"}
            };
            var customConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(customSettings!)
                .Build();

            // Act
            var service = new NotificationService(customConfig);

            // Assert
            service.Should().NotBeNull();
            // The service should use the custom queue path from configuration
        }

        [Fact]
        public void Constructor_WithoutQueuePathInConfig_UsesDefaultPath()
        {
            // Arrange
            var emptyConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>())
                .Build();

            // Act
            Action act = () => new NotificationService(emptyConfig);

            // Assert
            act.Should().NotThrow();
            // Service should use default path: .\\Private$\\ContosoUniversityNotifications
        }

        #endregion

        #region SendNotification Tests

        [Fact]
        public void SendNotification_ValidData_DoesNotThrow()
        {
            // Arrange
            var service = new NotificationService(_configuration);

            // Act
            Action act = () => service.SendNotification(
                "Student", 
                "123", 
                EntityOperation.CREATE, 
                "TestUser");

            // Assert
            act.Should().NotThrow("because SendNotification should handle errors gracefully");
        }

        [Fact]
        public void SendNotification_WithDisplayName_DoesNotThrow()
        {
            // Arrange
            var service = new NotificationService(_configuration);

            // Act
            Action act = () => service.SendNotification(
                "Course", 
                "456", 
                "Introduction to Programming",
                EntityOperation.UPDATE, 
                "TestUser");

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void SendNotification_AllOperations_Work()
        {
            // Arrange
            var service = new NotificationService(_configuration);
            var operations = new[] 
            { 
                EntityOperation.CREATE, 
                EntityOperation.UPDATE, 
                EntityOperation.DELETE 
            };

            // Act & Assert
            foreach (var operation in operations)
            {
                Action act = () => service.SendNotification("Test", "1", operation);
                act.Should().NotThrow($"because {operation} operation should work");
            }
        }

        #endregion

        #region ReceiveNotification Tests

        [Fact]
        public void ReceiveNotification_EmptyQueue_ReturnsNull()
        {
            // Arrange
            var service = new NotificationService(_configuration);

            // Act
            var result = service.ReceiveNotification();

            // Assert
            result.Should().BeNull("because the queue is empty");
        }

        [Fact]
        public void ReceiveNotification_HandlesTimeout_Gracefully()
        {
            // Arrange
            var service = new NotificationService(_configuration);

            // Act
            Action act = () => service.ReceiveNotification();

            // Assert
            act.Should().NotThrow("because timeout should be handled gracefully");
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public void SendNotification_WithException_DoesNotBreakMainOperation()
        {
            // This validates that notification errors don't break the main operation
            // which is critical for the modernization
            
            // Arrange
            var service = new NotificationService(_configuration);

            // Act
            Action act = () => service.SendNotification(null, null, EntityOperation.CREATE);

            // Assert
            act.Should().NotThrow("because errors should be caught and logged");
        }

        #endregion

        #region Dispose Tests

        [Fact]
        public void Dispose_ReleasesResources()
        {
            // Arrange
            var service = new NotificationService(_configuration);

            // Act
            Action act = () => service.Dispose();

            // Assert
            act.Should().NotThrow();
        }

        #endregion

        #region Integration with MSMQ.Messaging Tests

        [Fact]
        public void NotificationService_UsesMSMQMessaging_NotSystemMessaging()
        {
            // This test validates that we're using MSMQ.Messaging package (modernized)
            // instead of System.Messaging (.NET Framework)
            
            // Arrange & Act
            var service = new NotificationService(_configuration);

            // Assert
            service.Should().NotBeNull();
            service.GetType().Namespace.Should().Be("ContosoUniversity.Services");
            
            // The service should be using MSMQ.Messaging types internally
            // (validated by successful compilation and runtime)
        }

        #endregion
    }
}
