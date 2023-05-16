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

    /// <summary>
    /// Registers a user by user name, email and password.
    /// </summary>
    /// <param name="request"> Name, email and password in DTO class.</param>
    /// <returns>Ok if user successfully registered.</returns>
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

    /// <summary>
    /// Return a JWT token if user successfully logged in.
    /// </summary>
    /// <param name="request"> Email and password in DTO class.</param>
    /// <returns> JWT token with ok code.</returns>
    [HttpPost("login")]
    public ActionResult<string> LoginAsync(UserAuthDto request)
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

    /// <summary>
    /// Creates a JWT token for a user.
    /// </summary>
    /// <param name="user"> User for token creation.</param>
    /// <returns> Created JWT.</returns>
    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Auth:JWTkey"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(10),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
