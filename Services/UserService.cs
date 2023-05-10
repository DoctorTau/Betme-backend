using BetMe.Database;
using BetMe.Models;
using Microsoft.EntityFrameworkCore;

namespace BetMe.Services;

public class UserService : IUserService
{
    AppDbContext _dbContext;

    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }
        return user;
    }

    public async Task<List<Bet>> GetAllBetsOfUserAsync(int userId)
    {
        List<int> betIds = await _dbContext.UserBets.Where(ub => ub.UserId == userId)
                                                    .Select(ub => ub.BetId)
                                                    .ToListAsync();
        List<Bet> bets = await _dbContext.Bets.Where(b => betIds.Contains(b.Id))
                                              .ToListAsync();
        return bets;
    }

    public async Task AddWinToUsersAsync(Bet bet, Outcome winner)
    {
        List<UserBet> userBets = _dbContext.UserBets.Where(ub => ub.BetId == bet.Id)
                                                    .ToList();
        foreach (UserBet userBet in userBets)
        {
            if (userBet.OutcomeId == winner.Id)
            {
                User user = await GetUserByIdAsync(userBet.UserId);

                user.NumberOfWins++;
                _dbContext.Users.Update(user);
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<User> UpdateUserAsync(int id, User user)
    {
        User? userToUpdate = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (userToUpdate == null)
        {
            throw new ArgumentException("User not found");
        }

        userToUpdate.CopyFrom(user);
        await _dbContext.SaveChangesAsync();
        return userToUpdate;
    }

    public async Task<User> DeleteUserAsync(int userId)
    {
        User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }
}