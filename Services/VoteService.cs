using BetMe.Database;
using BetMe.Models;
using Microsoft.EntityFrameworkCore;

namespace BetMe.Services;

public class VoteService : IVoteService
{
    private readonly AppDbContext _dbContext;
    private readonly IBetService _betService;
    private readonly IOutcomeService _outcomeService;

    public VoteService(AppDbContext dbContext, IBetService betService, IOutcomeService outcomeService)
    {
        _dbContext = dbContext;
        _betService = betService;
        _outcomeService = outcomeService;
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

        Bet? bet = await _betService.GetBetByIdAsync(userBetDto.BetId);
        if (bet.Status != BetStatus.Open)
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

        return await _betService.GetBetByIdAsync(userBetDto.BetId);
    }

    public Task<Boolean> HasUserVotedAsync(int betId, int userId)
    {
        UserBet? userBet = _dbContext.UserBets.FirstOrDefault(ub => ub.BetId == betId && ub.UserId == userId);
        if (userBet == null)
        {
            throw new ArgumentException("UserBet not found.");
        }

        return Task.FromResult(userBet.HasVoted);
    }

    public async Task FinishBetVoting(int betId)
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
        await _outcomeService.SetWinner(bet);
    }

    public bool CheckVotingInBet(Bet bet)
    {
        if (bet.Status != BetStatus.Open)
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