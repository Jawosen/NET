using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;
    private readonly ICommentRepository commentRepo;

    public PostsController(IPostRepository postRepo, IUserRepository userRepo, ICommentRepository commentRepo)
    {
        this.postRepo = postRepo;
        this.userRepo = userRepo;
        this.commentRepo = commentRepo;
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost([FromBody] CreatePostDto request)
    {
        try
        {
            if (!userRepo.GetMany().Any(u => u.Id == request.UserId))
            {
                return BadRequest($"User with ID '{request.UserId}' does not exist.");
            }

            Post post = new()
            {
                Title = request.Title,
                Body = request.Body,
                UserId = request.UserId
            };

            Post created = await postRepo.AddAsync(post);
            return Created($"/posts/{created.Id}", ToDto(created));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PostDto>> UpdatePost([FromRoute] int id, [FromBody] UpdatePostDto request)
    {
        try
        {
            Post post = await postRepo.GetSingleAsync(id);
            post.Title = request.Title;
            post.Body = request.Body;
            await postRepo.UpdateAsync(post);

            return Ok(ToDto(post));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetSinglePost([FromRoute] int id)
    {
        try
        {
            Post post = await postRepo.GetSingleAsync(id);
            return Ok(ToDto(post));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetManyPosts(
        [FromQuery] string? title,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        try
        {
            IQueryable<Post> query = postRepo.GetMany();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(p => p.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
            }

            if (userId is not null)
            {
                query = query.Where(p => p.UserId == userId);
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                List<int> userIds = userRepo.GetMany()
                    .Where(u => u.Username.Equals(userName, StringComparison.OrdinalIgnoreCase))
                    .Select(u => u.Id)
                    .ToList();

                query = query.Where(p => userIds.Contains(p.UserId));
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
    public async Task<IActionResult> DeletePost([FromRoute] int id)
    {
        try
        {
            await postRepo.GetSingleAsync(id);
            await postRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id:int}/comments")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsOfPost([FromRoute] int id)
    {
        try
        {
            await postRepo.GetSingleAsync(id);

            List<CommentDto> comments = commentRepo.GetMany()
                .Where(c => c.PostId == id)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Body = c.Body,
                    Created = c.Created,
                    UserId = c.UserId,
                    PostId = c.PostId
                })
                .ToList();

            return Ok(comments);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    private static PostDto ToDto(Post post) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Body = post.Body,
        Created = post.Created,
        UserId = post.UserId
    };
}