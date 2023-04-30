using System.IdentityModel.Tokens.Jwt;
using BetMe.Models;
using BetMe.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetMe.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BetController : ControllerBase
{
    private IBetService _betService;
    public BetController(IBetService betService)
    {
        _betService = betService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBetsAsync()
    {
        List<Bet> bets = await _betService.GetAllBetsAsync();
        return Ok(bets);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBetByIdAsync(int id)
    {
        try
        {
            Bet bet = await _betService.GetBetByIdAsync(id);
            return Ok(bet);
        }
        catch (ArgumentException)
        {
            return NotFound("Bet not found.");
        }
    }

    [HttpPost, Authorize]
    public async Task<IActionResult> CreateBetAsync(BetCreatingDto bet)
    {
        int userId;
        try
        {
            int.TryParse(User?.FindFirst(c => c.Type.Contains("nameidentifier"))?.Value, out userId);
        }
        catch (Exception)
        {
            return BadRequest("Error while jwt parsing.");
        }
        Bet newBet = await _betService.CreateBetAsync(bet, userId);
        return Ok(newBet);
    }

}
