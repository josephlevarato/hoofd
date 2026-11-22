namespace Hoofd.Models;

using Microsoft.AspNetCore.Identity;

public class Horse : IdentityUser
{
    public DateOnly? Birthday { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}