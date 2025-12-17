using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("managers")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetManagers()
        {
            try
            {
                var managers = await _unitOfWork.Users.FindAsync(u => u.IsActive && u.Role == UserRole.Manager);
                var dtos = managers.Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive
                });
                return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(dtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<UserDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetUsers()
        {
            try
            {
                var users = await _unitOfWork.Users.FindAsync(u => u.IsActive);
                var dtos = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive
                });
                return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(dtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<UserDto>>.ErrorResponse(ex.Message));
            }
        }
    }
}
