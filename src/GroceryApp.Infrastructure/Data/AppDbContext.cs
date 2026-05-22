using GroceryApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GroceryApp.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<ShoppingList> ShoppingLists => Set<ShoppingList>();
    public DbSet<ShoppingItem> ShoppingItems => Set<ShoppingItem>();
    public DbSet<MasterItem> MasterItems => Set<MasterItem>();
    public DbSet<ShoppingHistory> ShoppingHistories => Set<ShoppingHistory>();
    public DbSet<ShoppingHistoryItem> ShoppingHistoryItems => Set<ShoppingHistoryItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasColumnName("id");
            e.Property(u => u.Username).HasColumnName("username").HasMaxLength(50).IsRequired();
            e.Property(u => u.PasswordHash).HasColumnName("password_hash");
            e.Property(u => u.CreatedAt).HasColumnName("created_at");
            e.HasIndex(u => u.Username).IsUnique();
        });

        modelBuilder.Entity<ShoppingList>(e =>
        {
            e.ToTable("shopping_lists");
            e.HasKey(l => l.Id);
            e.Property(l => l.Id).HasColumnName("id");
            e.Property(l => l.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            e.Property(l => l.UserId).HasColumnName("user_id");
            e.Property(l => l.CreatedAt).HasColumnName("created_at");
            e.HasMany(l => l.Items)
             .WithOne()
             .HasForeignKey(i => i.ListId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ShoppingItem>(e =>
        {
            e.ToTable("shopping_items");
            e.HasKey(i => i.Id);
            e.Property(i => i.Id).HasColumnName("id");
            e.Property(i => i.ListId).HasColumnName("list_id");
            e.Property(i => i.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            e.Property(i => i.Quantity).HasColumnName("quantity").HasMaxLength(50);
            e.Property(i => i.Memo).HasColumnName("memo");
            e.Property(i => i.IsChecked).HasColumnName("is_checked");
            e.Property(i => i.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<MasterItem>(e =>
        {
            e.ToTable("master_items");
            e.HasKey(m => m.Id);
            e.Property(m => m.Id).HasColumnName("id");
            e.Property(m => m.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            e.Property(m => m.Memo).HasColumnName("memo");
            e.Property(m => m.UserId).HasColumnName("user_id");
            e.Property(m => m.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<ShoppingHistory>(e =>
        {
            e.ToTable("shopping_histories");
            e.HasKey(h => h.Id);
            e.Property(h => h.Id).HasColumnName("id");
            e.Property(h => h.UserId).HasColumnName("user_id");
            e.Property(h => h.CompletedAt).HasColumnName("completed_at");
            e.HasMany(h => h.Items)
             .WithOne()
             .HasForeignKey(i => i.HistoryId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ShoppingHistoryItem>(e =>
        {
            e.ToTable("shopping_history_items");
            e.HasKey(i => i.Id);
            e.Property(i => i.Id).HasColumnName("id");
            e.Property(i => i.HistoryId).HasColumnName("history_id");
            e.Property(i => i.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            e.Property(i => i.Quantity).HasColumnName("quantity").HasMaxLength(50);
            e.Property(i => i.Memo).HasColumnName("memo");
            e.Property(i => i.IsChecked).HasColumnName("is_checked");
        });
    }
}
