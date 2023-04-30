using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BetMe.Models;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private IConfiguration _config;

    private static User user { get; set; }

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

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
    public ActionResult<string> Login(UserAuthDto request)
    {
        string errorMessage = "Invalid email or password";
        if (user == null)
        {
            return BadRequest(errorMessage);
        }

        bool validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (validPassword)
        {
            return Ok(CreateToken(user));
        }
        else
        {
            return BadRequest(errorMessage);
        }
    }

    // Create JWT token
    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Auth:JWTkey"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
