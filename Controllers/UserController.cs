using BetMe.Models;
using BetMe.Services;
using Microsoft.AspNetCore.Mvc;

namespace BetMe.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        List<User> users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserByIdAsync(int id)
    {
        try
        {
            User user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }
        catch (ArgumentException)
        {
            return NotFound("User not found.");
        }
    }

    [HttpGet("{id}/bets")]
    public async Task<IActionResult> GetAllBetsOfUserAsync(int id)
    {
        try
        {
            List<Bet> bets = await _userService.GetAllBetsOfUserAsync(id);
            return Ok(bets);
        }
        catch (ArgumentException)
        {
            return NotFound("User not found.");
        }
    }
}

