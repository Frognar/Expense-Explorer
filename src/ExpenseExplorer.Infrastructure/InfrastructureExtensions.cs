using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Infrastructure.Data;
using ExpenseExplorer.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseExplorer.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<IStoreRepository, Repository>()
            .AddScoped<IItemRepository, Repository>()
            .AddScoped<ICategoryRepository, Repository>()
            .AddScoped<IReceiptCommandRepository, Repository>();
    }

    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        string connectionString)
    {
        return services
            .AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(connectionString))
            .AddSingleton<DbInitializer>();
    }

    public static async Task InitializeDatabase(this IServiceProvider services)
    {
        await services.GetRequiredService<DbInitializer>().InitializeAsync();
    }
}