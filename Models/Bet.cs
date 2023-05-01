namespace BetMe.Models;

public class Bet
{
    public int Id { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CreatorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ClosedAt { get; set; }
    public BetStatus Status { get; set; }
    public int WinOutcomeId { get; set; } = -1;

    public Bet(string name, string description, DateTime closedAt, int creatorId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        if (closedAt < DateTime.UtcNow)
            throw new ArgumentException("ClosedAt cannot be in the past.", nameof(closedAt));

        Name = name;
        Description = description;
        ClosedAt = closedAt;
        CreatorId = creatorId;
        CreatedAt = DateTime.UtcNow;
        Status = BetStatus.Creating;
    }
}