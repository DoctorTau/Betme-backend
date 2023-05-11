using BetMe.Database;
using BetMe.Models;
using Microsoft.EntityFrameworkCore;

namespace BetMe.Services;

public class BetService : IBetService
{
    private readonly AppDbContext _dbContext;
    private readonly IOutcomeService _outcomeService;

    public BetService(AppDbContext dbContext, IOutcomeService outcomeService)
    {
        _dbContext = dbContext;
        _outcomeService = outcomeService;
    }

    public async Task<Bet> CreateBetAsync(BetCreatingDto bet, long userId)
    {
        Bet newBet = new(bet.Name,
                         bet.Description,
                         userId);

        await _dbContext.Bets.AddAsync(newBet);
        await _dbContext.SaveChangesAsync();

        return newBet;
    }

    public async Task<List<Bet>> GetAllBetsAsync()
    {
        var bets = await _dbContext.Bets.ToListAsync();
        return bets;
    }

    public Task<Bet> GetBetByIdAsync(long id)
    {
        Bet? bet = _dbContext.Bets.FirstOrDefault(b => b.Id == id);
        if (bet == null)
        {
            throw new ArgumentException("Bet not found.");
        }
        return Task.FromResult(bet);
    }

    public Task<List<UserBet>> GetAllUsersOfBetAsync(long betId)
    {
        List<UserBet> userBets = _dbContext.UserBets.Where(ub => ub.BetId == betId)
                                               .ToList();
        return Task.FromResult(userBets);
    }

    public Task<UserBet> AddUserToBetAsync(UserBetDto userBet)
    {
        UserBet ub = new UserBet { BetId = userBet.BetId, UserId = userBet.UserId, OutcomeId = userBet.OutcomeId };
        _dbContext.UserBets.Add(ub);
        // Increment Selections to outcome.
        Outcome? outcome = _dbContext.Outcomes.FirstOrDefault(o => o.Id == userBet.OutcomeId);
        if (outcome == null)
        {
            throw new ArgumentException("Outcome not found.");
        }
        outcome.Selections++;
        _dbContext.Outcomes.Update(outcome);
        _dbContext.SaveChanges();

        return Task.FromResult(ub);
    }

    public async Task<Bet> StartBetAsync(long betId)
    {
        Bet? bet = await _dbContext.Bets.FirstOrDefaultAsync(b => b.Id == betId);
        if (bet == null)
        {
            throw new ArgumentException("Bet not found.");
        }

        await DeleteUnselectedOutcomesAsync(bet);

        await _outcomeService.AddOutcomeAsync(new OutcomeDto
        {
            BetId = bet.Id,
            Name = "Ни один из исходов" // "None of the outcomes"
        });

        bet.Status = BetStatus.Open;


        await _dbContext.SaveChangesAsync();
        return bet;
    }

    private async Task DeleteUnselectedOutcomesAsync(Bet bet)
    {
        List<Outcome> outcomes = _dbContext.Outcomes.Where(o => o.BetId == bet.Id)
                                                   .ToList();
        List<Outcome> outcomesToDelete = new();
        foreach (Outcome outcome in outcomes)
        {
            if (outcome.Selections == 0)
            {
                outcomesToDelete.Add(outcome);
            }
        }

        _dbContext.Outcomes.RemoveRange(outcomesToDelete);
        await _dbContext.SaveChangesAsync();
    }


    public Task<Bet> DeleteBetAsync(long betId)
    {
        Bet? bet = _dbContext.Bets.FirstOrDefault(b => b.Id == betId);
        if (bet == null)
        {
            throw new ArgumentException("Bet not found.");
        }

        _dbContext.Bets.Remove(bet);
        _dbContext.SaveChanges();

        return Task.FromResult(bet);
    }
}