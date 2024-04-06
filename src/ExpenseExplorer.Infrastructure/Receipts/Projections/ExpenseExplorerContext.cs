namespace ExpenseExplorer.Infrastructure.Receipts.Projections;

using ExpenseExplorer.Infrastructure.Receipts.Projections.Models;
using Microsoft.EntityFrameworkCore;

internal sealed class ExpenseExplorerContext(string connectionString) : DbContext
{
  private readonly string _connectionString = connectionString;

  public DbSet<DbReceiptHeader> ReceiptHeaders { get; set; } = default!;

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseNpgsql(_connectionString);
  }
}
