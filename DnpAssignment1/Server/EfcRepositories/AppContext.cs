using Entities;
using Microsoft.EntityFrameworkCore;

namespace EfcRepositories;

public class AppContext : DbContext
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Relative to the working directory. "../EfcRepositories/app.db" resolves to the same file
        // whether you run from the EfcRepositories folder (dotnet ef) or the WebAPI folder (dotnet run).
        optionsBuilder.UseSqlite("Data Source=../EfcRepositories/app.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
    }
}
