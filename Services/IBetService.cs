using BetMe.Models;

namespace BetMe.Services;

public interface IBetService
{
    /// <summary>
    /// Gets the list with all bets.
    /// </summary>
    /// <returns> List with all bets.</returns>
    Task<List<Bet>> GetAllBetsAsync();
    /// <summary>
    /// Gets a bet by id.
    /// </summary>
    /// <param name="id">Id of finding bet.</param>
    /// <returns> Bet with provided id.</returns>
    Task<Bet> GetBetByIdAsync(int id);
    /// <summary>
    /// Gets all participants of the bet.
    /// </summary>
    /// <param name="betId"> Id of the bet.</param>
    /// <returns>List of participants.</returns>
    Task<List<UserBet>> GetAllUsersOfBetAsync(int betId);
    /// <summary>
    /// Creates a new bet.
    /// </summary>
    /// <param name="bet"> Bet params.</param>
    /// <param name="userId"> Creator. </param>
    /// <returns> Created bet.</returns>
    Task<Bet> CreateBetAsync(BetCreatingDto bet, int userId);
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
    Task<List<Outcome>> GetAllOutcomesOfBetAsync(int betId);
    /// <summary>
    /// Adds a participant to the bet.
    /// </summary>
    /// <param name="userBet"> Participant params.</param>
    /// <returns> Created link.</returns>
    Task<UserBet> AddUserToBetAsync(UserBetDto userBet);
    /// <summary>
    /// Starts the bet.
    /// </summary>
    /// <param name="betId"> Bet to start.</param>
    /// <returns> Started bet.</returns>
    Task<Bet> StartBetAsync(int betId);
    /// <summary>
    /// Finishes the bet and starts the voting.
    /// </summary>
    /// <param name="betId"> Bet to finish id.</param>
    /// <returns> Bet on stage of voting. </returns>
    Task<Bet> FinishBetAsync(int betId);
    /// <summary>
    /// Makes a vote.
    /// </summary>
    /// <param name="userBetDto"> A user, bet and outcome id of the vote.</param>
    /// <returns> The Bet where vote were made.</returns>
    Task<Bet> VoteAsync(UserBetDto userBetDto);
    /// <summary>
    /// Checks if the user has voted.
    /// </summary>
    /// <param name="userId"> User to check.</param>
    /// <param name="betId"> Bet to check in.</param>
    /// <returns> True if the user has voted and false otherwise.</returns>
    Task<Boolean> HasUserVotedAsync(int betId, int userId);
    /// <summary>
    /// Deletes the bet.
    /// </summary>
    /// <param name="betId"> Id of bet to delete.</param>
    /// <returns> Deleted bet.</returns>
    Task<Bet> DeleteBetAsync(int betId);
}