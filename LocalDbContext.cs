using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SalesCommissionsAPI.Models;

public class LocalDbContext : IdentityDbContext<IdentityUser>
{
    public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    public DbSet<SMSS> SMSS { get; set; }
    public DbSet<ActionLog> ActionLogs { get; set; } // Add the ActionLogs DbSet


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Define composite key for SMSS entity
        modelBuilder.Entity<SMSS>()
            .HasKey(s => new { s.Salesrep, s.Year, s.Cmth });
    }
}
