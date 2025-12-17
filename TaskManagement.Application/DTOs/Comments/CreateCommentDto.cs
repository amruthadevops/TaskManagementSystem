using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.DTOs.Comments
{
    public class CreateCommentDto
    {
        public string Content { get; set; } = string.Empty;
        public int TaskId { get; set; }
    }
}