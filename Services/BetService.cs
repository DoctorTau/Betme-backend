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
        Bet newBet = new(bet.Name,
                         bet.Description,
                         DateTime.Now.AddDays(7).ToUniversalTime(), // 7 days from now. In future we will add ability to change this.
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

    public Task<Bet> GetBetByIdAsync(int id)
    {
        Bet? bet = _dbContext.Bets.FirstOrDefault(b => b.Id == id);
        if (bet == null)
        {
            throw new ArgumentException("Bet not found.");
        }
        return Task.FromResult(bet);
    }

    public Task<List<UserBet>> GetAllUsersOfBetAsync(int betId)
    {
        List<UserBet> userBets = _dbContext.UserBets.Where(ub => ub.BetId == betId)
                                               .ToList();
        return Task.FromResult(userBets);
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

    public async Task<Bet> StartBetAsync(int betId)
    {
        Bet? bet = await _dbContext.Bets.FirstOrDefaultAsync(b => b.Id == betId);
        if (bet == null)
        {
            throw new ArgumentException("Bet not found.");
        }

        DeleteUnselectedOutcomes(bet);

        await AddOutcomeAsync(new OutcomeDto
        {
            BetId = bet.Id,
            Name = "Ни один из исходов" // "None of the outcomes"
        });

        if (DateTime.Now > bet.ClosedAt)
        {
            bet.Status = BetStatus.Closed;
            await _dbContext.SaveChangesAsync();
            throw new ArgumentException("You can't start a bet after the closing time.");
        }

        bet.Status = BetStatus.Open;


        await _dbContext.SaveChangesAsync();
        return bet;
    }

    public async Task<Bet> FinishBetAsync(int betId)
    {
        Bet? bet = _dbContext.Bets.FirstOrDefault(b => b.Id == betId);
        if (bet == null)
        {
            throw new ArgumentException("Bet not found.");
        }

        bet.Status = BetStatus.Voting;
        _dbContext.Bets.Update(bet);
        await _dbContext.SaveChangesAsync();

        return bet;
    }

    public async Task<Bet> VoteAsync(UserBetDto userBetDto)
    {
        UserBet? userBet = _dbContext.UserBets.FirstOrDefault(ub => ub.BetId == userBetDto.BetId && ub.UserId == userBetDto.UserId);
        if (userBet == null)
        {
            throw new ArgumentException("UserBet not found.");
        }

        Outcome? outcome = _dbContext.Outcomes.FirstOrDefault(o => o.Id == userBetDto.OutcomeId);
        if (outcome == null)
        {
            throw new ArgumentException("Outcome not found.");
        }

        Bet? bet = await GetBetByIdAsync(userBetDto.BetId);
        if (bet.Status != BetStatus.Voting && bet.Status != BetStatus.Open)
        {
            throw new ArgumentException("Bet is not in voting phase.");
        }

        if (userBet.HasVoted)
        {
            throw new ArgumentException("User has already voted.");
        }

        userBet.HasVoted = true;
        outcome.Votes++;

        _dbContext.Outcomes.Update(outcome);
        _dbContext.UserBets.Update(userBet);
        await _dbContext.SaveChangesAsync();

        if (CheckVotingInBet(bet))
        {
            await FinishBetVoting(bet.Id);
        }

        return await GetBetByIdAsync(userBetDto.BetId);
    }

    private void DeleteUnselectedOutcomes(Bet bet)
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
        _dbContext.SaveChanges();
    }

    private async Task FinishBetVoting(int betId)
    {
        Bet? bet = await _dbContext.Bets.FirstOrDefaultAsync(b => b.Id == betId);
        if (bet == null)
        {
            throw new ArgumentException("Bet not found.");
        }

        if (CheckVotingInBet(bet))
        {
            bet.Status = BetStatus.Closed;
        }
        await SetWinner(bet);
    }

    private async Task SetWinner(Bet bet)
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
        await _dbContext.SaveChangesAsync();
    }

    public Task<Bet> DeleteBetAsync(int betId)
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

    private bool CheckVotingInBet(Bet bet)
    {
        if (bet.Status != BetStatus.Voting && bet.Status != BetStatus.Open)
        {
            return false;
        }

        List<UserBet> userBets = _dbContext.UserBets.Where(ub => ub.BetId == bet.Id)
                                                    .ToList();
        foreach (UserBet userBet in userBets)
        {
            if (!userBet.HasVoted)
            {
                return false;
            }
        }

        return true;
    }
}