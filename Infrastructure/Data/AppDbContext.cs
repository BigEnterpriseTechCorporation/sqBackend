using System.Net.Mime;
using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Exercise> Exercises { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Unit>().HasMany<Exercise>(e => e.Exercises).WithOne(x => x.Unit).HasForeignKey(x => x.UnitId);
        modelBuilder.Entity<Unit>().HasOne<User>(u => u.Owner).WithMany(x => x.Units).HasForeignKey(u => u.OwnerId);
        modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();
    }
}