using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IRepository<User>? _users;
        private IRepository<Team>? _teams;
        private IRepository<TeamMember>? _teamMembers;
        private IRepository<TaskItem>? _tasks;
        private IRepository<Comment>? _comments;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<User> Users =>
            _users ??= new Repository<User>(_context);

        public IRepository<Team> Teams =>
            _teams ??= new Repository<Team>(_context);

        public IRepository<TeamMember> TeamMembers =>
            _teamMembers ??= new Repository<TeamMember>(_context);

        public IRepository<TaskItem> Tasks =>
            _tasks ??= new Repository<TaskItem>(_context);

        public IRepository<Comment> Comments =>
            _comments ??= new Repository<Comment>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}