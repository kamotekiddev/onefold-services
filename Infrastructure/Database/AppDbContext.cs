using Application.Abstractions.Persistence;
using Domain.Entities;
using Domain.Entities.Workout;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users { get; set; }
    public DbSet<Credential> Credentials { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Exercise> Exercises { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}