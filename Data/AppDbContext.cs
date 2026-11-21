using Microsoft.EntityFrameworkCore;
using Hoofd.Models;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Horse> Horses => Set<Horse>();
}
