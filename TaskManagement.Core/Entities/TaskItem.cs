using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TaskManagement.Core.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? DueDate { get; set; }
        public int CreatedById { get; set; }
        public int? AssignedToId { get; set; }
        public int? TeamId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual User CreatedBy { get; set; } = null!;
        public virtual User? AssignedTo { get; set; }
        public virtual Team? Team { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

    public enum TaskStatus
    {
        ToDo = 1,
        InProgress = 2,
        Done = 3
    }

    public enum TaskPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }
}