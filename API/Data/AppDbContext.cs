using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Data;
public class AppDbContext : DbContext
    {
    public AppDbContext(DbContextOptions<AppDbContext>options):base(options)
    {
        
    }
    protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
            Id = 1,
            Name = "rami",
            });
        modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
            Id = 2,
            Name = "mohammad",
            });
        }
    public DbSet<AppUser> appUsers { get; set; }
    }
