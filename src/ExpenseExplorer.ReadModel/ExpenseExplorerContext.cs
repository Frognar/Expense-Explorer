namespace ExpenseExplorer.ReadModel;

using ExpenseExplorer.ReadModel.Models.Persistence;
using Microsoft.EntityFrameworkCore;

public sealed class ExpenseExplorerContext(string connectionString) : DbContext
{
  private readonly string _connectionString = connectionString;

  public DbSet<DbReceipt> Receipts { get; set; } = default!;

  public DbSet<DbPurchase> Purchases { get; set; } = default!;

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseNpgsql(_connectionString);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);
    modelBuilder.Entity<DbReceipt>()
      .HasMany(r => r.Purchases)
      .WithOne()
      .HasForeignKey(p => p.ReceiptId)
      .IsRequired();
  }
}
