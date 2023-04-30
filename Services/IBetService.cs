using BetMe.Models;

namespace BetMe.Services;

public interface IBetService
{
    Task<List<Bet>> GetAllBetsAsync();
    Task<Bet> GetBetByIdAsync(int id);
    Task<Bet> CreateBetAsync(BetCreatingDto bet, int userId);
}