using System;
using System.Linq;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Check if database has been seeded
            if (context.Users.Any())
            {
                return; // DB has been seeded
            }

            // Seed Users
            var users = new[]
            {
                new User
                {
                    Email = "admin@taskmanager.com",
                    FirstName = "Admin",
                    LastName = "User",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "manager@taskmanager.com",
                    FirstName = "Manager",
                    LastName = "User",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
                    Role = UserRole.Manager,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "user@taskmanager.com",
                    FirstName = "Regular",
                    LastName = "User",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                    Role = UserRole.User,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            Console.WriteLine("Database seeded successfully!");
            Console.WriteLine("Admin: admin@taskmanager.com / Admin@123");
            Console.WriteLine("Manager: manager@taskmanager.com / Manager@123");
            Console.WriteLine("User: user@taskmanager.com / User@123");
        }
    }
}