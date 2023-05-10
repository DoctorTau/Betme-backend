namespace BetMe.Models;

public class Outcome
{
    public long Id { get; set; }
    public String Name { get; set; } = string.Empty;
    public long BetId { get; set; }
    public int Selections { get; set; } = 0;
    public int Votes { get; set; } = 0;
}