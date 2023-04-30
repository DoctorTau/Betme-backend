namespace BetMe.Models;

public class Outcome
{
    public int Id { get; set; }
    public String Name { get; set; } = string.Empty;
    public int BetId { get; set; }
    public int Votes { get; set; }
}