using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Teams;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TeamDto>>>> GetAllTeams()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

                var teams = await _teamService.GetAllTeamsAsync(userId, userRole);
                return Ok(ApiResponse<IEnumerable<TeamDto>>.SuccessResponse(teams));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<TeamDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TeamDto>>> GetTeamById(int id)
        {
            try
            {
                var team = await _teamService.GetTeamByIdAsync(id);
                return Ok(ApiResponse<TeamDto>.SuccessResponse(team));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<TeamDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<TeamDto>>> CreateTeam([FromBody] CreateTeamDto createTeamDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

                var team = await _teamService.CreateTeamAsync(createTeamDto, userId, userRole);
                return CreatedAtAction(nameof(GetTeamById), new { id = team.Id },
                    ApiResponse<TeamDto>.SuccessResponse(team, "Team created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<TeamDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("{teamId}/members/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<object>>> AddMember(int teamId, int userId)
        {
            try
            {
                var managerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _teamService.AddMemberToTeamAsync(teamId, userId, managerId);
                return Ok(ApiResponse<object?>.SuccessResponse(null, "Member added successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{teamId}/members/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<object>>> RemoveMember(int teamId, int userId)
        {
            try
            {
                var managerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _teamService.RemoveMemberFromTeamAsync(teamId, userId, managerId);
                return Ok(ApiResponse<object?>.SuccessResponse(null, "Member removed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}