using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.Comments;
using TaskManagement.Application.Interfaces;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CommentDto> AddCommentAsync(CreateCommentDto dto, int userId)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(dto.TaskId);
            if (task == null)
                throw new Exception("Task not found");

            var comment = new Comment
            {
                Content = dto.Content,
                TaskId = dto.TaskId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Comments.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            return await GetCommentDtoAsync(comment.Id);
        }

        public async Task<IEnumerable<CommentDto>> GetTaskCommentsAsync(int taskId)
        {
            var comments = await _unitOfWork.Comments.FindAsync(c => c.TaskId == taskId);

            var commentDtos = new List<CommentDto>();
            foreach (var comment in comments.OrderBy(c => c.CreatedAt))
            {
                var user = await _unitOfWork.Users.GetByIdAsync(comment.UserId);
                commentDtos.Add(new CommentDto
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    TaskId = comment.TaskId,
                    UserId = comment.UserId,
                    UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
                    CreatedAt = comment.CreatedAt
                });
            }

            return commentDtos;
        }

        public async Task DeleteCommentAsync(int commentId, int userId)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
            if (comment == null)
                throw new Exception("Comment not found");

            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own comments");

            await _unitOfWork.Comments.DeleteAsync(comment);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<CommentDto> GetCommentDtoAsync(int commentId)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
            if (comment == null)
                throw new Exception("Comment not found");

            var user = await _unitOfWork.Users.GetByIdAsync(comment.UserId);

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                TaskId = comment.TaskId,
                UserId = comment.UserId,
                UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
                CreatedAt = comment.CreatedAt
            };
        }
    }
}