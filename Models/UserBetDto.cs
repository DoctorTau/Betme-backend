namespace BetMe.Models;


public class UserBetDto
{
    public required int BetId { get; set; }
    public required int UserId { get; set; }
    public required int OutcomeId { get; set; }
}