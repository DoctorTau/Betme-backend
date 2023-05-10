using BetMe.Database;
using BetMe.Models;
using Microsoft.EntityFrameworkCore;

namespace BetMe.Services;

public class OutcomeService : IOutcomeService
{
    private readonly AppDbContext _dbContext;
    private readonly IUserService _userService;

    public OutcomeService(AppDbContext dbContext, IUserService userService)
    {
        _dbContext = dbContext;
        _userService = userService;
    }

    public async Task<Outcome> AddOutcomeAsync(OutcomeDto outcomeDto)
    {
        Outcome outcome = new()
        {
            BetId = outcomeDto.BetId,
            Name = outcomeDto.Name
        };

        await _dbContext.Outcomes.AddAsync(outcome);
        await _dbContext.SaveChangesAsync();

        return outcome;
    }

    public async Task<List<Outcome>> GetAllOutcomesOfBetAsync(long betId)
    {
        List<Outcome> outcomes = await _dbContext.Outcomes.Where(o => o.BetId == betId)
                                                          .ToListAsync();
        return outcomes;
    }

    public async Task SetWinner(Bet bet)
    {
        List<Outcome> outcomes = await GetAllOutcomesOfBetAsync(bet.Id);
        outcomes = outcomes.OrderByDescending(o => o.Votes)
                           .ToList();
        if (outcomes.Count == 0)
        {
            throw new ArgumentException("No outcomes found.");
        }

        Outcome winner = outcomes[0];
        bet.WinOutcomeId = winner.Id;

        _dbContext.Bets.Update(bet);

        if (winner.Name != "Ни один из исходов")
            await _userService.AddWinToUsersAsync(bet, winner);

        await _dbContext.SaveChangesAsync();
    }



}