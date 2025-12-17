using System;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Team> Teams { get; }
        IRepository<TeamMember> TeamMembers { get; }
        IRepository<TaskItem> Tasks { get; }
        IRepository<Comment> Comments { get; }
        Task<int> SaveChangesAsync();
    }
}