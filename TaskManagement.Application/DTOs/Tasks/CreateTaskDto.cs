using System;

namespace TaskManagement.Application.DTOs.Tasks
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; } = 2;
        public DateTime? DueDate { get; set; }
        public int? AssignedToId { get; set; }
        public int? TeamId { get; set; }
    }
}
