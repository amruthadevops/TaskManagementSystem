using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public async Task SendTaskAssignmentNotificationAsync(string email, string taskTitle)
        {
            // Mock email notification
            _logger.LogInformation($"[EMAIL] Task Assignment Notification sent to {email}");
            _logger.LogInformation($"Subject: New Task Assigned - {taskTitle}");
            _logger.LogInformation($"Body: You have been assigned a new task: {taskTitle}");

            await Task.CompletedTask;
        }

        public async Task SendTaskStatusChangeNotificationAsync(string email, string taskTitle, string newStatus)
        {
            // Mock email notification
            _logger.LogInformation($"[EMAIL] Task Status Change Notification sent to {email}");
            _logger.LogInformation($"Subject: Task Status Updated - {taskTitle}");
            _logger.LogInformation($"Body: Task '{taskTitle}' status changed to {newStatus}");

            await Task.CompletedTask;
        }
    }
}