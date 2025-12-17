using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.Teams;
using TaskManagement.Application.Interfaces;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.Application.Services
{
    public class TeamService : ITeamService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TeamService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TeamDto> CreateTeamAsync(CreateTeamDto dto, int userId, string userRole)
        {
            if (userRole != "Admin" && userRole != "Manager")
                throw new UnauthorizedAccessException("Only Admins and Managers can create teams");

            var managerId = dto.ManagerId ?? userId;

            var team = new Team
            {
                Name = dto.Name,
                Description = dto.Description,
                ManagerId = managerId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Teams.AddAsync(team);
            await _unitOfWork.SaveChangesAsync();

            return await GetTeamByIdAsync(team.Id);
        }

        public async Task<TeamDto> GetTeamByIdAsync(int id)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(id);

            if (team == null)
                throw new Exception("Team not found");

            // Get manager details
            var manager = await _unitOfWork.Users.GetByIdAsync(team.ManagerId);

            // Get team members
            var teamMembers = await _unitOfWork.TeamMembers
                .FindAsync(tm => tm.TeamId == id);

            var memberDtos = new List<TeamMemberDto>();
            foreach (var tm in teamMembers)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(tm.UserId);
                if (user != null)
                {
                    memberDtos.Add(new TeamMemberDto
                    {
                        UserId = tm.UserId,
                        Name = $"{user.FirstName} {user.LastName}",
                        Email = user.Email,
                        JoinedAt = tm.JoinedAt
                    });
                }
            }

            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                ManagerId = team.ManagerId,
                ManagerName = manager != null ? $"{manager.FirstName} {manager.LastName}" : "Unknown",
                CreatedAt = team.CreatedAt,
                Members = memberDtos
            };
        }

        public async Task<IEnumerable<TeamDto>> GetAllTeamsAsync(int userId, string userRole)
        {
            IEnumerable<Team> teams;

            if (userRole == "Admin")
            {
                teams = await _unitOfWork.Teams.FindAsync(t => t.IsActive);
            }
            else if (userRole == "Manager")
            {
                teams = await _unitOfWork.Teams.FindAsync(t => t.ManagerId == userId && t.IsActive);
            }
            else // User
            {
                // Get teams where user is a member
                var userTeamMemberships = await _unitOfWork.TeamMembers
                    .FindAsync(tm => tm.UserId == userId);

                var teamIds = userTeamMemberships.Select(tm => tm.TeamId).ToList();

                teams = await _unitOfWork.Teams.FindAsync(t => teamIds.Contains(t.Id) && t.IsActive);
            }

            var teamDtos = new List<TeamDto>();

            foreach (var team in teams)
            {
                var manager = await _unitOfWork.Users.GetByIdAsync(team.ManagerId);
                var teamMembers = await _unitOfWork.TeamMembers.FindAsync(tm => tm.TeamId == team.Id);

                var memberDtos = new List<TeamMemberDto>();
                foreach (var tm in teamMembers)
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(tm.UserId);
                    if (user != null)
                    {
                        memberDtos.Add(new TeamMemberDto
                        {
                            UserId = tm.UserId,
                            Name = $"{user.FirstName} {user.LastName}",
                            Email = user.Email,
                            JoinedAt = tm.JoinedAt
                        });
                    }
                }

                teamDtos.Add(new TeamDto
                {
                    Id = team.Id,
                    Name = team.Name,
                    Description = team.Description,
                    ManagerId = team.ManagerId,
                    ManagerName = manager != null ? $"{manager.FirstName} {manager.LastName}" : "Unknown",
                    CreatedAt = team.CreatedAt,
                    Members = memberDtos
                });
            }

            return teamDtos;
        }

        public async Task AddMemberToTeamAsync(int teamId, int userId, int managerId)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(teamId);
            if (team == null)
                throw new Exception("Team not found");

            if (team.ManagerId != managerId)
                throw new UnauthorizedAccessException("Only team manager can add members");

            var existingMember = await _unitOfWork.TeamMembers
                .ExistsAsync(tm => tm.TeamId == teamId && tm.UserId == userId);

            if (existingMember)
                throw new Exception("User is already a team member");

            var teamMember = new TeamMember
            {
                TeamId = teamId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            };

            await _unitOfWork.TeamMembers.AddAsync(teamMember);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveMemberFromTeamAsync(int teamId, int userId, int managerId)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(teamId);
            if (team == null)
                throw new Exception("Team not found");

            if (team.ManagerId != managerId)
                throw new UnauthorizedAccessException("Only team manager can remove members");

            var members = await _unitOfWork.TeamMembers
                .FindAsync(tm => tm.TeamId == teamId && tm.UserId == userId);

            var member = members.FirstOrDefault();
            if (member == null)
                throw new Exception("User is not a team member");

            await _unitOfWork.TeamMembers.DeleteAsync(member);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}