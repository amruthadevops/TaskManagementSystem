using System;

namespace TaskManagement.Application.DTOs.Tasks
{
    public class UpdateTaskDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
        public int? Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public int? AssignedToId { get; set; }
        public int? TeamId { get; set; }
    }
}