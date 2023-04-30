using BetMe.Models;

namespace BetMe.Services;

public interface IUserService
{
    /// <summary>
    /// Gets the list with all users.
    /// </summary>
    /// <returns> List of all users.</returns>
    Task<List<User>> GetAllUsersAsync();

    /// <summary>
    /// Gets a user by id.
    /// </summary>
    /// <param name="id">Id of user to find.</param>
    /// <returns>User with provided id.</returns>
    Task<User> GetUserByIdAsync(int id);

    /// <summary>
    /// Gets all bets of the user.
    /// </summary>
    /// <param name="userId"> User id.</param>
    /// <returns> List of all bets where user takes part in.</returns>
    Task<List<Bet>> GetAllBetsOfUserAsync(int userId);
}