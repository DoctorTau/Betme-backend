using BetMe.Models;

namespace BetMe.Services;

public interface IVoteService
{
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
    Task<Boolean> HasUserVotedAsync(long betId, long userId);
    /// <summary>
    /// Finishes the voting in the bet.
    /// </summary>
    /// <param name="betId"> Id of bet to finish.</param>
    /// <returns></returns>
    Task FinishBetVoting(long betId);
    /// <summary>
    /// Checks if all users have voted in the bet.
    /// </summary>
    /// <param name="bet"> Bet to check in.</param>
    /// <returns> True if all users finished voting, false otherwise</returns>
    bool CheckVotingInBet(Bet bet);
}