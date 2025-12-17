using System.Threading.Tasks;

namespace TaskManagement.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendTaskAssignmentNotificationAsync(string email, string taskTitle);
        Task SendTaskStatusChangeNotificationAsync(string email, string taskTitle, string newStatus);
    }
}