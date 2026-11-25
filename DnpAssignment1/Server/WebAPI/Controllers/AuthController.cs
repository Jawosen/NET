using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public AuthController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequest request)
    {
        try
        {
            User? user = await userRepo.GetMany()
                .FirstOrDefaultAsync(u => u.Username.ToLower() == request.UserName.ToLower());

            if (user is null)
            {
                return Unauthorized("User not found.");
            }

            if (user.Password != request.Password)
            {
                return Unauthorized("Incorrect password.");
            }

            UserDto dto = new()
            {
                Id = user.Id,
                UserName = user.Username
            };
            return Ok(dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}
