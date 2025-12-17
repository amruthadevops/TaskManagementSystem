using System;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.Dashboard;
using TaskManagement.Application.Interfaces;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardDto> GetDashboardStatsAsync(int userId, string userRole)
        {
            System.Collections.Generic.IEnumerable<TaskItem> tasks;

            if (userRole == "Admin")
            {
                tasks = await _unitOfWork.Tasks.GetAllAsync();
            }
            else if (userRole == "Manager")
            {
                var managerTasks = await _unitOfWork.Tasks
                    .FindAsync(t => t.CreatedById == userId || t.AssignedToId == userId);

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

            var taskList = tasks.ToList();

            return new DashboardDto
            {
                TotalTasks = taskList.Count,
                ToDoTasks = taskList.Count(t => t.Status == TaskManagement.Core.Entities.TaskStatus.ToDo),
                InProgressTasks = taskList.Count(t => t.Status == TaskManagement.Core.Entities.TaskStatus.InProgress),
                DoneTasks = taskList.Count(t => t.Status == TaskManagement.Core.Entities.TaskStatus.Done),
                OverdueTasks = taskList.Count(t =>
                    t.DueDate.HasValue &&
                    t.DueDate.Value < DateTime.UtcNow &&
                    t.Status != TaskManagement.Core.Entities.TaskStatus.Done),
                HighPriorityTasks = taskList.Count(t =>
                    t.Priority == TaskPriority.High ||
                    t.Priority == TaskPriority.Critical)
            };
        }
    }
}