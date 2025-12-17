using System.Threading.Tasks;
using TaskManagement.Application.DTOs.Dashboard;

namespace TaskManagement.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardStatsAsync(int userId, string userRole);
    }
}