using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            if (!await userRepo.GetMany().AnyAsync(u => u.Id == request.UserId))
            {
                return BadRequest($"User with ID '{request.UserId}' does not exist.");
            }

            Post post = new()
            {
                Title = request.Title,
                Body = request.Body,
                UserId = request.UserId,
                Created = DateTime.UtcNow
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
    public async Task<ActionResult<PostDto>> GetSinglePost(
        [FromRoute] int id,
        [FromQuery] bool includeAuthor,
        [FromQuery] bool includeComments)
    {
        try
        {
            // Builds one SQL statement; nothing is executed until FirstOrDefaultAsync()
            IQueryable<Post> queryForPost = postRepo.GetMany()
                .Where(p => p.Id == id);

            if (includeAuthor)
            {
                queryForPost = queryForPost.Include(p => p.User);
            }

            if (includeComments)
            {
                queryForPost = queryForPost.Include(p => p.Comments);
            }

            PostDto? dto = await queryForPost
                .Select(post => new PostDto
                {
                    Id = post.Id,
                    Title = post.Title,
                    Body = post.Body,
                    Created = post.Created,
                    UserId = post.UserId,
                    Author = includeAuthor
                        ? new UserDto
                        {
                            Id = post.User.Id,
                            UserName = post.User.Username
                        }
                        : null,
                    Comments = includeComments
                        ? post.Comments.Select(c => new CommentDto
                        {
                            Id = c.Id,
                            Body = c.Body,
                            Created = c.Created,
                            UserId = c.UserId,
                            PostId = c.PostId
                        }).ToList()
                        : new List<CommentDto>()
                })
                .FirstOrDefaultAsync();

            return dto is null ? NotFound($"Post with ID '{id}' was not found.") : Ok(dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetManyPosts(
        [FromQuery] string? title,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        try
        {
            IQueryable<Post> query = postRepo.GetMany();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(p => p.Title.ToLower().Contains(title.ToLower()));
            }

            if (userId is not null)
            {
                query = query.Where(p => p.UserId == userId);
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                // Navigation property: EFC joins the Users table for us
                query = query.Where(p => p.User.Username.ToLower() == userName.ToLower());
            }

            List<PostDto> posts = await query
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Body = p.Body,
                    Created = p.Created,
                    UserId = p.UserId
                })
                .ToListAsync();

            return Ok(posts);
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

            List<CommentDto> comments = await commentRepo.GetMany()
                .Where(c => c.PostId == id)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Body = c.Body,
                    Created = c.Created,
                    UserId = c.UserId,
                    PostId = c.PostId
                })
                .ToListAsync();

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