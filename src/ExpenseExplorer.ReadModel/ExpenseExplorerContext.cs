namespace ExpenseExplorer.ReadModel;

using ExpenseExplorer.ReadModel.Models;
using Microsoft.EntityFrameworkCore;

public sealed class ExpenseExplorerContext(string connectionString) : DbContext
{
  private readonly string _connectionString = connectionString;

  public DbSet<DbReceiptHeader> ReceiptHeaders { get; set; } = default!;

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseNpgsql(_connectionString);
  }
}
