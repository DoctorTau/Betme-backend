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
}