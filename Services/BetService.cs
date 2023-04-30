using BetMe.Database;
using BetMe.Models;
using Microsoft.EntityFrameworkCore;

namespace BetMe.Services;

public class BetService : IBetService
{
    private readonly AppDbContext _dbContext;

    public BetService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Bet> CreateBetAsync(BetCreatingDto bet, int userId)
    {
        Bet newBet = new Bet
        {
            Name = bet.Name,
            Description = bet.Description,
            ClosedAt = bet.ClosedAt,
            CreatorId = userId,
            CreatedAt = DateTime.UtcNow,
            Status = BetStatus.Creating
        };

        await _dbContext.Bets.AddAsync(newBet);
        await _dbContext.SaveChangesAsync();

        return newBet;
    }

    public async Task<List<Bet>> GetAllBetsAsync()
    {
        var bets = await _dbContext.Bets.ToListAsync();
        return bets;
    }

    public Task<Bet> GetBetByIdAsync(int id)
    {
        Bet? bet = _dbContext.Bets.FirstOrDefault(b => b.Id == id);
        if (bet == null)
        {
            throw new ArgumentException("Bet not found.");
        }
        return Task.FromResult(bet);
    }

    public Task<List<User>> GetAllUsersOfBetAsync(int betId)
    {
        List<int> userIds = _dbContext.UserBets.Where(ub => ub.BetId == betId)
                                               .Select(ub => ub.UserId)
                                               .ToList();
        List<User> users = _dbContext.Users.Where(u => userIds.Contains(u.Id))
                                           .ToList();
        return Task.FromResult(users);
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

    public async Task<List<Outcome>> GetAllOutcomesOfBetAsync(int betId)
    {
        List<Outcome> outcomes = await _dbContext.Outcomes.Where(o => o.BetId == betId)
                                                          .ToListAsync();
        return outcomes;
    }

    public Task<UserBet> AddUserToBetAsync(UserBet userBet)
    {
        _dbContext.UserBets.Add(userBet);
        _dbContext.SaveChanges();

        return Task.FromResult(userBet);
    }
}