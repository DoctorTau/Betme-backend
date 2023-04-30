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

    /// <summary>
    /// Gets the list with all users.
    /// </summary>
    /// <returns> List of users.</returns>
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    /// <summary>
    /// Gets a user by id.
    /// </summary>
    /// <param name="id"> Id of users.</param>
    /// <returns>User with specified id.</returns>
    public async Task<User> GetUserByIdAsync(int id)
    {
        User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }
        return user;
    }
}