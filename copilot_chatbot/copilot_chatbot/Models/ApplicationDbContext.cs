using copilot_chatbot.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Import> Imports { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "botProjectDatabase.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
