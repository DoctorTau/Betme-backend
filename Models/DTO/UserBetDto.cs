namespace BetMe.Models;


public class UserBetDto
{
    public required long BetId { get; set; }
    public required long UserId { get; set; }
    public required long OutcomeId { get; set; }
}