using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public TaskService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, int createdById)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = (TaskPriority)dto.Priority,
                DueDate = dto.DueDate,
                AssignedToId = dto.AssignedToId,
                TeamId = dto.TeamId,
                CreatedById = createdById,
                Status = TaskManagement.Core.Entities.TaskStatus.ToDo,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

            // Send notification
            if (dto.AssignedToId.HasValue)
            {
                var assignedUser = await _unitOfWork.Users.GetByIdAsync(dto.AssignedToId.Value);
                if (assignedUser != null)
                {
                    await _notificationService.SendTaskAssignmentNotificationAsync(
                        assignedUser.Email, task.Title);
                }
            }

            return await GetTaskDtoAsync(task.Id);
        }

        public async Task<TaskDto> GetTaskByIdAsync(int id, int userId, string userRole)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);

            if (task == null)
                throw new Exception("Task not found");

            if (!await CanAccessTaskAsync(task, userId, userRole))
                throw new UnauthorizedAccessException("Access denied");

            return await MapToDtoAsync(task);
        }

        public async Task<IEnumerable<TaskDto>> GetAllTasksAsync(int userId, string userRole)
        {
            IEnumerable<TaskItem> tasks;

            if (userRole == "Admin")
            {
                tasks = await _unitOfWork.Tasks.GetAllAsync();
            }
            else if (userRole == "Manager")
            {
                // Get tasks created by manager or assigned to manager
                var managerTasks = await _unitOfWork.Tasks
                    .FindAsync(t => t.CreatedById == userId || t.AssignedToId == userId);

                // Get tasks in teams managed by this manager
                var managedTeams = await _unitOfWork.Teams
                    .FindAsync(t => t.ManagerId == userId);
                var managedTeamIds = managedTeams.Select(t => t.Id).ToList();

                var teamTasks = await _unitOfWork.Tasks
                    .FindAsync(t => t.TeamId.HasValue && managedTeamIds.Contains(t.TeamId.Value));

                tasks = managerTasks.Union(teamTasks).Distinct();
            }
            else // User
            {
                tasks = await _unitOfWork.Tasks
                    .FindAsync(t => t.AssignedToId == userId || t.CreatedById == userId);
            }

            var taskDtos = new List<TaskDto>();
            foreach (var task in tasks.OrderByDescending(t => t.CreatedAt))
            {
                taskDtos.Add(await MapToDtoAsync(task));
            }

            return taskDtos;
        }

        public async Task<IEnumerable<TaskDto>> GetUserTasksAsync(int userId)
        {
            // Get tasks directly assigned to user
            var directTasks = await _unitOfWork.Tasks.FindAsync(t => t.AssignedToId == userId);

            // Get teams where user is a member
            var userTeams = await _unitOfWork.TeamMembers.FindAsync(tm => tm.UserId == userId);
            var teamIds = userTeams.Select(tm => tm.TeamId).ToList();

            // Get tasks assigned to those teams
            var teamTasks = teamIds.Any()
                ? await _unitOfWork.Tasks.FindAsync(t => t.TeamId.HasValue && teamIds.Contains(t.TeamId.Value))
                : new List<TaskItem>();

            // Combine and deduplicate
            var allTasks = directTasks.Union(teamTasks).Distinct().OrderByDescending(t => t.CreatedAt);

            var taskDtos = new List<TaskDto>();
            foreach (var task in allTasks)
            {
                taskDtos.Add(await MapToDtoAsync(task));
            }

            return taskDtos;
        }

        public async Task<TaskDto> UpdateTaskAsync(int id, UpdateTaskDto dto, int userId, string userRole)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
                throw new Exception("Task not found");

            if (!await CanModifyTaskAsync(task, userId, userRole))
                throw new UnauthorizedAccessException("Access denied");

            var oldStatus = task.Status;

            if (dto.Title != null) task.Title = dto.Title;
            if (dto.Description != null) task.Description = dto.Description;
            if (dto.Status.HasValue) task.Status = (TaskManagement.Core.Entities.TaskStatus)dto.Status.Value;
            if (dto.Priority.HasValue) task.Priority = (TaskPriority)dto.Priority.Value;
            if (dto.DueDate.HasValue) task.DueDate = dto.DueDate;
            
            // Handle AssignedToId - null means clear it
            if (dto.AssignedToId.HasValue)
                task.AssignedToId = dto.AssignedToId.Value > 0 ? dto.AssignedToId : null;
            
            // Handle TeamId - null means clear it
            if (dto.TeamId.HasValue)
                task.TeamId = dto.TeamId.Value > 0 ? dto.TeamId : null;

            task.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Tasks.UpdateAsync(task);
            await _unitOfWork.SaveChangesAsync();

            // Send status change notification
            if (dto.Status.HasValue && oldStatus != task.Status && task.AssignedToId.HasValue)
            {
                var assignedUser = await _unitOfWork.Users.GetByIdAsync(task.AssignedToId.Value);
                if (assignedUser != null)
                {
                    await _notificationService.SendTaskStatusChangeNotificationAsync(
                        assignedUser.Email, task.Title, task.Status.ToString());
                }
            }

            return await GetTaskDtoAsync(id);
        }

        public async Task DeleteTaskAsync(int id, int userId, string userRole)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
                throw new Exception("Task not found");

            if (!await CanModifyTaskAsync(task, userId, userRole))
                throw new UnauthorizedAccessException("Access denied");

            await _unitOfWork.Tasks.DeleteAsync(task);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskDto>> FilterTasksAsync(int userId, string userRole,
            int? status, int? priority, bool? overdue)
        {
            var tasks = await GetAllTasksAsync(userId, userRole);
            var filteredTasks = tasks.AsEnumerable();

            if (status.HasValue)
            {
                var statusEnum = (TaskManagement.Core.Entities.TaskStatus)status.Value;
                filteredTasks = filteredTasks.Where(t => t.Status == statusEnum.ToString());
            }

            if (priority.HasValue)
            {
                var priorityEnum = (TaskPriority)priority.Value;
                filteredTasks = filteredTasks.Where(t => t.Priority == priorityEnum.ToString());
            }

            if (overdue.HasValue && overdue.Value)
            {
                filteredTasks = filteredTasks.Where(t =>
                    t.DueDate.HasValue &&
                    t.DueDate.Value < DateTime.UtcNow &&
                    t.Status != "Done");
            }

            return filteredTasks.ToList();
        }

        private async Task<TaskDto> GetTaskDtoAsync(int taskId)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
            if (task == null)
                throw new Exception("Task not found");

            return await MapToDtoAsync(task);
        }

        private async Task<bool> CanAccessTaskAsync(TaskItem task, int userId, string userRole)
        {
            if (userRole == "Admin") return true;
            if (task.CreatedById == userId || task.AssignedToId == userId) return true;

            if (userRole == "Manager" && task.TeamId.HasValue)
            {
                var team = await _unitOfWork.Teams.GetByIdAsync(task.TeamId.Value);
                return team?.ManagerId == userId;
            }

            return false;
        }

        private Task<bool> CanModifyTaskAsync(TaskItem task, int userId, string userRole)
        {
            if (userRole == "Admin") return Task.FromResult(true);
            if (userRole == "Manager") return Task.FromResult(true);
            if (task.CreatedById == userId) return Task.FromResult(true);

            return Task.FromResult(false);
        }

        private async Task<TaskDto> MapToDtoAsync(TaskItem task)
        {
            var createdBy = await _unitOfWork.Users.GetByIdAsync(task.CreatedById);
            User? assignedTo = null;
            Team? team = null;

            if (task.AssignedToId.HasValue)
            {
                assignedTo = await _unitOfWork.Users.GetByIdAsync(task.AssignedToId.Value);
            }

            if (task.TeamId.HasValue)
            {
                team = await _unitOfWork.Teams.GetByIdAsync(task.TeamId.Value);
            }

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                CreatedById = task.CreatedById,
                CreatedByName = createdBy != null ? $"{createdBy.FirstName} {createdBy.LastName}" : "Unknown",
                AssignedToId = task.AssignedToId,
                AssignedToName = assignedTo != null ? $"{assignedTo.FirstName} {assignedTo.LastName}" : null,
                TeamId = task.TeamId,
                TeamName = team?.Name,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }
}