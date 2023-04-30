using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BetMe.Models;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{

    [HttpPost("register")]
    public IActionResult Register(UserRegistrationDto request)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        user = new User
        {
            Id = 0,
            Name = request.Name,
            Email = request.Email,
            PasswordHash = hashedPassword,
            RegistrationDate = DateTime.Now
        };

        return Ok(user);
    }

    [HttpPost("login")]
    public IActionResult Login(UserAuthDto request)
    {
        string errorMessage = "Invalid email or password";
        if (user == null)
        {
            return BadRequest(errorMessage);
        }

        bool validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (validPassword)
        {
            return Ok(user);
        }
        else
        {
            return BadRequest(errorMessage);
        }
    }
}
