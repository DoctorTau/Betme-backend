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
        try
        {
            int userId = GetUserIdFromJwt();

            Bet newBet = await _betService.CreateBetAsync(bet, userId);
            return Ok(newBet);
        }
        catch (ArgumentException)
        {
            return NotFound("User not found.");
        }
    }

    [HttpPost("outcome"), Authorize]
    public async Task<IActionResult> AddOutcomeAsync(OutcomeDto outcomeDto)
    {
        Outcome outcome = await _betService.AddOutcomeAsync(outcomeDto);
        return Ok(outcome);
    }

    [HttpGet("{id}/participants")]
    public async Task<IActionResult> GetAllUsersOfBetAsync(int id)
    {
        List<User> users = await _betService.GetAllUsersOfBetAsync(id);
        return Ok(users);
    }

    [HttpGet("{id}/outcomes")]
    public async Task<IActionResult> GetAllOutcomesOfBetAsync(int id)
    {
        List<Outcome> outcomes = await _betService.GetAllOutcomesOfBetAsync(id);
        return Ok(outcomes);
    }

    [HttpGet("{id}/join"), Authorize]
    public async Task<IActionResult> JoinBetAsync(UserBet userBet)
    {
        try
        {
            int userId = GetUserIdFromJwt();

            await _betService.AddUserToBetAsync(userBet);
            return Ok();
        }
        catch (ArgumentException)
        {
            return NotFound("User not found.");
        }
    }

    [HttpPost("{id}/start"), Authorize]
    public async Task<IActionResult> StartBetAsync(int id)
    {
        try
        {
            int userId = GetUserIdFromJwt();
            Bet bet = await _betService.GetBetByIdAsync(id);
            if (bet.CreatorId != userId)
            {
                return Unauthorized("You are not the creator of the bet.");
            }

            bet = await _betService.StartBetAsync(id);

            return Ok(bet);
        }
        catch (ArgumentException)
        {
            return NotFound("Bet not found.");
        }
    }

    private int GetUserIdFromJwt()
    {
        int userId;
        int.TryParse(User?.FindFirst(c => c.Type.Contains("nameidentifier"))?.Value, out userId);
        return userId;
    }
}
