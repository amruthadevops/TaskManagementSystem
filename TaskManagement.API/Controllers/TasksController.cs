using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TaskDto>>>> GetAllTasks()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

                var tasks = await _taskService.GetAllTasksAsync(userId, userRole);
                return Ok(ApiResponse<IEnumerable<TaskDto>>.SuccessResponse(tasks));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<TaskDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TaskDto>>> GetTaskById(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

                var task = await _taskService.GetTaskByIdAsync(id, userId, userRole);
                return Ok(ApiResponse<TaskDto>.SuccessResponse(task));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<TaskDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("my-tasks")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TaskDto>>>> GetMyTasks()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var tasks = await _taskService.GetUserTasksAsync(userId);
                return Ok(ApiResponse<IEnumerable<TaskDto>>.SuccessResponse(tasks));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<TaskDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<TaskDto>>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var task = await _taskService.CreateTaskAsync(createTaskDto, userId);
                return CreatedAtAction(nameof(GetTaskById), new { id = task.Id },
                    ApiResponse<TaskDto>.SuccessResponse(task, "Task created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<TaskDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<TaskDto>>> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

                var task = await _taskService.UpdateTaskAsync(id, updateTaskDto, userId, userRole);
                return Ok(ApiResponse<TaskDto>.SuccessResponse(task, "Task updated successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<TaskDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteTask(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

                await _taskService.DeleteTaskAsync(id, userId, userRole);
                return Ok(ApiResponse<object?>.SuccessResponse(null, "Task deleted successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TaskDto>>>> FilterTasks(
            [FromQuery] int? status, [FromQuery] int? priority, [FromQuery] bool? overdue)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

                var tasks = await _taskService.FilterTasksAsync(userId, userRole, status, priority, overdue);
                return Ok(ApiResponse<IEnumerable<TaskDto>>.SuccessResponse(tasks));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<TaskDto>>.ErrorResponse(ex.Message));
            }
        }
    }
}
