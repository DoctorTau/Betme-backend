using System.ComponentModel.DataAnnotations;

namespace BetMe.Models;

public class BetCreatingDto
{
    [Required, MaxLength(256)]
    public required string Name { get; set; }
    [Required, MaxLength(1024)]
    public required string Description { get; set; }
    [Required, DataType(DataType.DateTime)]
    public DateTime ClosedAt { get; set; }
}