using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Interfaces
{
    public interface ITaskService
    {
        Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, int createdById);
        Task<TaskDto> GetTaskByIdAsync(int id, int userId, string userRole);
        Task<IEnumerable<TaskDto>> GetAllTasksAsync(int userId, string userRole);
        Task<IEnumerable<TaskDto>> GetUserTasksAsync(int userId);
        Task<TaskDto> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, int userId, string userRole);
        Task DeleteTaskAsync(int id, int userId, string userRole);
        Task<IEnumerable<TaskDto>> FilterTasksAsync(int userId, string userRole, int? status, int? priority, bool? overdue);
    }
}