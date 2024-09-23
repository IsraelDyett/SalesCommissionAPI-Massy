using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SalesCommissionsAPI.Models;

namespace SalesCommissionsAPI
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // public DbSet<SMSS> SMSS { get; set; }
        public DbSet<SalesRep> SalesRep { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //// Define composite key for SMSS entity
            //modelBuilder.Entity<SMSS>()
            //    .HasKey(s => new { s.Salesrep, s.Year, s.Cmth });

            // If needed, define the composite key or other configurations for SalesRep
            modelBuilder.Entity<SalesRep>()
             .HasKey(sr => new { sr.SlsRepId }); // Example of composite key definition
        }
    }
}
