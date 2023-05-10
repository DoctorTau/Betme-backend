using BetMe.Models;
using BetMe.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetMe.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VoteController : ControllerBase
{
    private IVoteService _voteService;

    public VoteController(IVoteService voteService)
    {
        _voteService = voteService;
    }

    [HttpPut("vote"), Authorize]
    public async Task<IActionResult> VoteAsync(UserBetDto userBetDto)
    {
        try
        {
            await _voteService.VoteAsync(userBetDto);
            return Ok();
        }
        catch (ArgumentException e)
        {
            return NotFound(e.Message);
        }
    }

    /// <summary>
    /// Checks if the user has voted for the given bet.
    /// </summary>
    /// <param name="betId"> Bet to check in.</param>
    /// <returns>True if user have voted and false otherwise</returns>
    [HttpGet("{betId}/checkVote/{userId}"), Authorize]
    public async Task<IActionResult> CheckIfUserVotedAsync(int betId, int userId)
    {
        try
        {
            bool voted = await _voteService.HasUserVotedAsync(betId, userId);
            return Ok(voted);
        }
        catch (ArgumentException)
        {
            return NotFound("Bet not found.");
        }
    }
}