namespace BetMe.Models;

public class Outcome
{
    public int Id { get; set; }
    public String Name { get; set; } = string.Empty;
    public int BetId { get; set; }
    public int Selections { get; set; } = 0;
    public int Votes { get; set; } = 0;
}