namespace Hoofd.Models;

public class Horse
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required DateOnly Birthday { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}