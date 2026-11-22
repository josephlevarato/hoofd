using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Hoofd.Models;

public class AppDbContext : IdentityDbContext<Horse>
{
    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b); // must be called first

        b.Entity<Horse>().ToTable("horses");
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) :
        base(options)
    { }
}
