using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BetMe.Models;

namespace BetMe.Database;
public class AppDbContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public AppDbContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to postgres with connection string from app settings
        options.UseNpgsql(Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                          ?? Configuration.GetConnectionString("DefaultConnection"));
    }

    public required DbSet<User> Users { get; set; }
    public required DbSet<Bet> Bets { get; set; }
    public required DbSet<Outcome> Outcomes { get; set; }
    public required DbSet<UserBet> UserBets { get; set; }
}