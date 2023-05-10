namespace BetMe.Models;

public class User
{
    public long Id { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; } = DateTime.Now.ToUniversalTime();
    public UserRole Role { get; set; } = UserRole.User;
    public int NumberOfWins { get; set; } = 0;

    public void CopyFrom(User user)
    {
        Name = user.Name;
        Email = user.Email;
        PasswordHash = user.PasswordHash;
        RegistrationDate = user.RegistrationDate;
        Role = user.Role;
        NumberOfWins = user.NumberOfWins;
    }
}