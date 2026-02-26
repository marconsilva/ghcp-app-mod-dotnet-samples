using ContosoUniversity.Services;
using ContosoUniversity.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace ContosoUniversity.UnitTests.TestUtilities
{
    /// <summary>
    /// Fake NotificationService for testing without MSMQ dependencies
    /// </summary>
    public class FakeNotificationService : NotificationService
    {
        public List<SentNotification> SentNotifications { get; } = new();
        public List<Notification> NotificationsToReturn { get; set; } = new();
        
        public FakeNotificationService() : base(CreateFakeConfiguration())
        {
        }

        private static IConfiguration CreateFakeConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"NotificationQueuePath", ".\\Private$\\TestQueue"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();
        }

        public new void SendNotification(string entityType, string entityId, EntityOperation operation, string userName = null)
        {
            SentNotifications.Add(new SentNotification
            {
                EntityType = entityType,
                EntityId = entityId,
                EntityDisplayName = null,
                Operation = operation,
                UserName = userName,
                Timestamp = DateTime.Now
            });
        }

        public new void SendNotification(string entityType, string entityId, string entityDisplayName, EntityOperation operation, string userName = null)
        {
            SentNotifications.Add(new SentNotification
            {
                EntityType = entityType,
                EntityId = entityId,
                EntityDisplayName = entityDisplayName,
                Operation = operation,
                UserName = userName,
                Timestamp = DateTime.Now
            });
        }

        public new Notification ReceiveNotification()
        {
            if (NotificationsToReturn.Count > 0)
            {
                var notification = NotificationsToReturn[0];
                NotificationsToReturn.RemoveAt(0);
                return notification;
            }
            return null;
        }

        public void ClearSentNotifications()
        {
            SentNotifications.Clear();
        }
    }

    public class SentNotification
    {
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string EntityDisplayName { get; set; }
        public EntityOperation Operation { get; set; }
        public string UserName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
