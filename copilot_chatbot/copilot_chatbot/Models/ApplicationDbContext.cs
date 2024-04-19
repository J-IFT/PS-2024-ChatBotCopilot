using copilot_chatbot.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Import> Imports { get; set; }
    // Ajoutez les autres DbSet pour les autres modèles

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "terrabloomDatabase.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}