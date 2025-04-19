using System.Net.Mime;
using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<UserSolution> UserSolutions { get; set; }
    public DbSet<UnitLike> UnitLikes { get; set; }
    public DbSet<ExerciseSolution> ExerciseSolutions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unit relationships
        modelBuilder.Entity<Unit>().HasMany<Exercise>(e => e.Exercises)
            .WithOne(x => x.Unit)
            .HasForeignKey(x => x.UnitId);
        
        modelBuilder.Entity<Unit>().HasOne<User>(u => u.Owner)
            .WithMany(x => x.Units)
            .HasForeignKey(u => u.OwnerId);
        
        // User relationships
        modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();
        
        // UnitLike relationships
        modelBuilder.Entity<UnitLike>()
            .HasOne(ul => ul.User)
            .WithMany(u => u.UnitLikes)
            .HasForeignKey(ul => ul.UserId);
        
        modelBuilder.Entity<UnitLike>()
            .HasOne(ul => ul.Unit)
            .WithMany(u => u.Likes)
            .HasForeignKey(ul => ul.UnitId);
        
        // UserSolution relationships
        modelBuilder.Entity<UserSolution>()
            .HasOne(us => us.User)
            .WithMany(u => u.Solutions)
            .HasForeignKey(us => us.UserId);
        
        modelBuilder.Entity<UserSolution>()
            .HasOne(us => us.Exercise)
            .WithMany(e => e.UserSolutions)
            .HasForeignKey(us => us.ExerciseId);
        
        // ExerciseSolution relationships
        modelBuilder.Entity<ExerciseSolution>()
            .HasOne(es => es.User)
            .WithMany()
            .HasForeignKey(es => es.UserId);
        
        modelBuilder.Entity<ExerciseSolution>()
            .HasOne(es => es.Exercise)
            .WithMany()
            .HasForeignKey(es => es.ExerciseId);
        
        // Configure BaseEntity properties for all entity types
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(t => t.ClrType.IsSubclassOf(typeof(BaseEntity))))
        {
            // Configure Id as primary key
            modelBuilder.Entity(entityType.ClrType).HasKey("Id");
            
            // Configure created/updated timestamps
            modelBuilder.Entity(entityType.ClrType).Property("CreatedAt").IsRequired();
            modelBuilder.Entity(entityType.ClrType).Property("UpdatedAt").IsRequired(false);
        }
    }
}