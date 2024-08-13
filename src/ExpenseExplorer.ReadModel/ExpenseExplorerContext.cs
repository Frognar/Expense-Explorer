using ExpenseExplorer.ReadModel.Models.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.ReadModel;

public sealed class ExpenseExplorerContext(string connectionString) : DbContext
{
  private readonly string _connectionString = connectionString;

  public DbSet<DbPosition> Positions { get; set; } = default!;

  public DbSet<DbReceipt> Receipts { get; set; } = default!;

  public DbSet<DbPurchase> Purchases { get; set; } = default!;

  public DbSet<DbIncome> Incomes { get; set; } = default!;

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

    modelBuilder.Entity<DbPosition>()
      .HasKey(p => new { p.CommitPosition, p.PreparePosition });
  }
}
