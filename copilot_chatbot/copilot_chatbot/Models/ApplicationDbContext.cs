using System;
using System.IO;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Import entity
        modelBuilder.Entity<Import>()
            .HasOne(i => i.Product)
            .WithMany(p => p.Imports)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Import>()
            .HasOne(i => i.User)
            .WithMany(u => u.Imports)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Export entity
        modelBuilder.Entity<Export>()
            .HasOne(e => e.User)
            .WithMany(u => u.Exports)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Export>()
            .HasMany(e => e.GeneratedDataProducts)
            .WithOne(g => g.Export)
            .HasForeignKey(g => g.ExportId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Keyword entity
        modelBuilder.Entity<Keyword>()
            .HasMany(k => k.ProductKeywords)
            .WithOne(pk => pk.Keyword)
            .HasForeignKey(pk => pk.KeywordId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Product entity
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Imports)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure ProductKeyword entity
        modelBuilder.Entity<ProductKeyword>()
            .HasOne(pk => pk.GeneratedDataProduct)
            .WithMany(g => g.ProductKeywords)
            .HasForeignKey(pk => pk.GeneratedDataProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductKeyword>()
            .HasOne(pk => pk.Keyword)
            .WithMany(k => k.ProductKeywords)
            .HasForeignKey(pk => pk.KeywordId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure User entity
        modelBuilder.Entity<User>()
            .HasMany(u => u.Imports)
            .WithOne(i => i.User)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);

    }

}
