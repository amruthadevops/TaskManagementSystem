using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.Comments;

namespace TaskManagement.Application.Interfaces
{
    public interface ICommentService
    {
        Task<CommentDto> AddCommentAsync(CreateCommentDto createCommentDto, int userId);
        Task<IEnumerable<CommentDto>> GetTaskCommentsAsync(int taskId);
        Task DeleteCommentAsync(int commentId, int userId);
    }
}