using Dapper;
using ExpenseExplorer.Application.Features.Receipts.AddItem;
using ExpenseExplorer.Application.Features.Receipts.CreateHeader;
using ExpenseExplorer.Application.Features.Receipts.DeleteHeader;
using ExpenseExplorer.Application.Features.Receipts.DeleteItem;
using ExpenseExplorer.Application.Features.Receipts.Duplicate;
using ExpenseExplorer.Application.Features.Receipts.GetReceipt;
using ExpenseExplorer.Application.Features.Receipts.GetReceipts;
using ExpenseExplorer.Application.Features.Receipts.UpdateHeader;
using ExpenseExplorer.Application.Features.Receipts.UpdateItem;
using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Infrastructure.Data;
using ExpenseExplorer.Infrastructure.Database;
using ExpenseExplorer.Infrastructure.Database.TypeHandlers;
using ExpenseExplorer.Infrastructure.Features.Receipts;
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
            .AddScoped<IReceiptCommandRepository, Repository>()
            .AddScoped<ReceiptRepository>()
            .AddScoped<ICreateReceiptHeaderPersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IUpdateReceiptHeaderPersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IDeleteReceiptHeaderPersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IDuplicateReceiptPersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IAddReceiptItemPersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IUpdateReceiptItemPersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IReceiptItemDeletePersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IGetReceiptByIdPersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IGetReceiptSummariesPersistence>(sp => sp.GetRequiredService<ReceiptRepository>());
    }

    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        string connectionString)
    {
        SqlMapper.AddTypeHandler(new DateOnlyHandler());
        return services
            .AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(connectionString))
            .AddSingleton<DbInitializer>();
    }

    public static async Task InitializeDatabase(this IServiceProvider services)
    {
        await services.GetRequiredService<DbInitializer>().InitializeAsync();
    }
}