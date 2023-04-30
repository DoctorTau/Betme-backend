using System.ComponentModel.DataAnnotations;

namespace BetMe.Models;

public class OutcomeDto
{
    [Required, MaxLength(256)]
    public String Name { get; set; } = string.Empty;
    [Required]
    public int BetId { get; set; }
}