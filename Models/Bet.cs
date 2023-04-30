namespace BetMe.Models;

public class Bet
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public User Creator { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime ClosedAt { get; set; }
    public BetStatus Status { get; set; }
}