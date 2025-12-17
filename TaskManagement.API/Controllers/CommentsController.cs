using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Comments;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("task/{taskId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CommentDto>>>> GetTaskComments(int taskId)
        {
            try
            {
                var comments = await _commentService.GetTaskCommentsAsync(taskId);
                return Ok(ApiResponse<IEnumerable<CommentDto>>.SuccessResponse(comments));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<CommentDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CommentDto>>> AddComment([FromBody] CreateCommentDto createCommentDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var comment = await _commentService.AddCommentAsync(createCommentDto, userId);
                return Ok(ApiResponse<CommentDto>.SuccessResponse(comment, "Comment added successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CommentDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteComment(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _commentService.DeleteCommentAsync(id, userId);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Comment deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}