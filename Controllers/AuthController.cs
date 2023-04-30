using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using BetMe.Database;
using BetMe.Models;

namespace BetMe.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private IConfiguration _config;
    private AppDbContext _dbContext;

    public AuthController(IConfiguration config, AppDbContext dbContext)
    {
        _config = config;
        _dbContext = dbContext;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(UserRegistrationDto request)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        User user = new()
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = hashedPassword,
            RegistrationDate = DateTime.Now.ToUniversalTime()
        };

        if (_dbContext.Users.Any(u => u.Email == user.Email))
        {
            return BadRequest("User with this email already exists.");
        }

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return Ok(user);
    }

    [HttpPost("login")]
    public ActionResult<string> Login(UserAuthDto request)
    {
        string errorMessage = "Invalid email or password";
        User? user = _dbContext.Users.FirstOrDefault(u => u.Email == request.Email);
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
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
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
