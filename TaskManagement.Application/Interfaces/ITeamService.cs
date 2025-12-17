using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.Teams;

namespace TaskManagement.Application.Interfaces
{
    public interface ITeamService
    {
        Task<TeamDto> CreateTeamAsync(CreateTeamDto createTeamDto, int userId, string userRole);
        Task<TeamDto> GetTeamByIdAsync(int id);
        Task<IEnumerable<TeamDto>> GetAllTeamsAsync(int userId, string userRole);
        Task AddMemberToTeamAsync(int teamId, int userId, int managerId);
        Task RemoveMemberFromTeamAsync(int teamId, int userId, int managerId);
    }
}