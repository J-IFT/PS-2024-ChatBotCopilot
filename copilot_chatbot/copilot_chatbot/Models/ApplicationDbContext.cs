using System;
using System.IO; // Ajout du namespace pour utiliser la classe Path
using copilot_chatbot.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Import> Imports { get; set; }
    public DbSet<Export> Exports { get; set; }
    public DbSet<GeneratedDataProduct> GeneratedDataProducts { get; set; }
    public DbSet<Keyword> Keywords { get; set; }
    public DbSet<ProductKeyword> ProductKeywords { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
   
        string databaseFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");

        if (!Directory.Exists(databaseFolderPath))
        {
            Directory.CreateDirectory(databaseFolderPath);
        }

        string dbPath = Path.Combine(databaseFolderPath, "terrabloomDatabase.db");

        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}