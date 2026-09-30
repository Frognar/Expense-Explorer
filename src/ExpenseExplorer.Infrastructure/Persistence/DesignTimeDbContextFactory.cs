using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ExpenseExplorer.Infrastructure.Persistence;

/// <summary>Used only by <c>dotnet ef</c> to create migrations; it never connects.</summary>
internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ExpenseExplorerDbContext>
{
    public ExpenseExplorerDbContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<ExpenseExplorerDbContext>()
            .UseNpgsql(
                "Host=localhost;Database=expense_explorer",
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", ExpenseExplorerDbContext.Schema))
            .Options);
}
