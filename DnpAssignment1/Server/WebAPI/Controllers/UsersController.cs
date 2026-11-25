using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public UsersController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser([FromBody] CreateUserDto request)
    {
        try
        {
            if (await userRepo.GetMany().AnyAsync(u => u.Username.ToLower() == request.UserName.ToLower()))
            {
                return BadRequest($"Username '{request.UserName}' is already taken.");
            }

            User user = new()
            {
                Username = request.UserName,
                Password = request.Password,
                Email = request.Email
            };

            User created = await userRepo.AddAsync(user);
            return Created($"/users/{created.Id}", ToDto(created));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> UpdateUser([FromRoute] int id, [FromBody] UpdateUserDto request)
    {
        try
        {
            User existing = await userRepo.GetSingleAsync(id);

            bool isChangingName = !existing.Username.Equals(request.UserName, StringComparison.OrdinalIgnoreCase);
            if (isChangingName && await userRepo.GetMany().AnyAsync(u => u.Username.ToLower() == request.UserName.ToLower()))
            {
                return BadRequest($"Username '{request.UserName}' is already taken.");
            }

            // Update the already-tracked entity instead of creating a new one with the same Id
            // (EFC refuses to track two instances with the same key).
            existing.Username = request.UserName;
            existing.Password = request.Password;
            existing.Email = request.Email;

            User updated = await userRepo.UpdateAsync(existing);
            return Ok(ToDto(updated));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetSingleUser([FromRoute] int id)
    {
        try
        {
            User user = await userRepo.GetSingleAsync(id);
            return Ok(ToDto(user));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetManyUsers([FromQuery] string? userName)
    {
        try
        {
            IQueryable<User> query = userRepo.GetMany();

            if (!string.IsNullOrWhiteSpace(userName))
            {
                query = query.Where(u => u.Username.ToLower().Contains(userName.ToLower()));
            }

            List<UserDto> users = await query
                .Select(u => new UserDto { Id = u.Id, UserName = u.Username })
                .ToListAsync();

            return Ok(users);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        try
        {
            await userRepo.GetSingleAsync(id);
            await userRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    private static UserDto ToDto(User user) => new()
    {
        Id = user.Id,
        UserName = user.Username
    };
}