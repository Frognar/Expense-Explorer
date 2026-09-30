using ExpenseExplorer.Application.Dictionaries;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Infrastructure.Dictionaries;
using ExpenseExplorer.Infrastructure.Persistence;
using ExpenseExplorer.Infrastructure.Receipts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseExplorer.Infrastructure;

public static class InfrastructureSetup
{
    public const string ConnectionStringName = "expense-explorer";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ExpenseExplorerDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IReceiptRepository, ReceiptRepository>();
        services.AddScoped<IReceiptQueries, ReceiptQueries>();
        services.AddScoped<IDictionaryQueries, DictionaryQueries>();
        return services;
    }

    /// <summary>Brings the database schema up to date. Safe on a database created by the previous app version.</summary>
    public static async Task MigrateDatabaseAsync(this IServiceProvider services)
    {
        await using AsyncServiceScope scope = services.CreateAsyncScope();
        ExpenseExplorerDbContext db = scope.ServiceProvider.GetRequiredService<ExpenseExplorerDbContext>();
        await db.Database.MigrateAsync();
    }
}
