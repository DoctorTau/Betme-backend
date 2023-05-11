using BetMe.Models;

namespace BetMe.Services;

public interface IOutcomeService
{
    /// <summary>
    /// Adds an outcome to the bet.
    /// </summary>
    /// <param name="outcomeDto"> Outcome params.</param>
    /// <returns> Created outcome.</returns>
    Task<Outcome> AddOutcomeAsync(OutcomeDto outcomeDto);
    /// <summary>
    /// Gets all outcomes of the bet.
    /// </summary>
    /// <param name="betId">Id of the bet.</param>
    /// <returns> List of outcomes of the bet.</returns>
    Task<List<Outcome>> GetAllOutcomesOfBetAsync(long betId);
    /// <summary>
    /// Sets the winner outcome to bet.
    /// </summary>
    /// <param name="bet"> Bet to set outcome.</param>
    /// <returns></returns>
    Task SetWinner(Bet bet);
}