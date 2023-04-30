namespace BetMe.Models;

public class User
{
    public int Id { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; } = DateTime.Now.ToUniversalTime();
    public int NumberOfWins { get; set; } = 0;
}