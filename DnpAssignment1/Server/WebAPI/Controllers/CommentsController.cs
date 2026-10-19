using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    private readonly IUserRepository userRepo;
    private readonly IPostRepository postRepo;

    public CommentsController(ICommentRepository commentRepo, IUserRepository userRepo, IPostRepository postRepo)
    {
        this.commentRepo = commentRepo;
        this.userRepo = userRepo;
        this.postRepo = postRepo;
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> AddComment([FromBody] CreateCommentDto request)
    {
        try
        {
            if (!userRepo.GetMany().Any(u => u.Id == request.UserId))
            {
                return BadRequest($"User with ID '{request.UserId}' does not exist.");
            }

            if (!postRepo.GetMany().Any(p => p.Id == request.PostId))
            {
                return BadRequest($"Post with ID '{request.PostId}' does not exist.");
            }

            Comment comment = new()
            {
                Body = request.Body,
                UserId = request.UserId,
                PostId = request.PostId,
                Created = DateTime.UtcNow
            };

            Comment created = await commentRepo.AddAsync(comment);
            return Created($"/comments/{created.Id}", ToDto(created));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CommentDto>> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentDto request)
    {
        try
        {
            Comment comment = await commentRepo.GetSingleAsync(id);
            comment.Body = request.Body;
            Comment updated = await commentRepo.UpdateAsync(comment);

            return Ok(ToDto(updated));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetSingleComment([FromRoute] int id)
    {
        try
        {
            Comment comment = await commentRepo.GetSingleAsync(id);
            return Ok(ToDto(comment));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetManyComments(
        [FromQuery] int? userId,
        [FromQuery] string? userName,
        [FromQuery] int? postId)
    {
        try
        {
            IQueryable<Comment> query = commentRepo.GetMany();

            if (userId is not null)
            {
                query = query.Where(c => c.UserId == userId);
            }

            if (postId is not null)
            {
                query = query.Where(c => c.PostId == postId);
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                List<int> userIds = userRepo.GetMany()
                    .Where(u => u.Username.Equals(userName, StringComparison.OrdinalIgnoreCase))
                    .Select(u => u.Id)
                    .ToList();

                query = query.Where(c => userIds.Contains(c.UserId));
            }

            return Ok(query.Select(ToDto).ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment([FromRoute] int id)
    {
        try
        {
            await commentRepo.GetSingleAsync(id);
            await commentRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    private static CommentDto ToDto(Comment comment) => new()
    {
        Id = comment.Id,
        Body = comment.Body,
        Created = comment.Created,
        UserId = comment.UserId,
        PostId = comment.PostId
    };
}