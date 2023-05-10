namespace BetMe.Models;

public class Bet
{
    public int Id { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CreatorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public BetStatus Status { get; set; }
    public int WinOutcomeId { get; set; } = -1;

    public Bet(string name, string description, int creatorId)
    {
        Name = name;
        Description = description;
        CreatorId = creatorId;
        CreatedAt = DateTime.UtcNow;
        Status = BetStatus.Creating;
    }
}