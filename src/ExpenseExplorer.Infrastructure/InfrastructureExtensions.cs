using Dapper;
using ExpenseExplorer.Application.Features.Categories.GetCategories;
using ExpenseExplorer.Application.Features.Items.GetItems;
using ExpenseExplorer.Application.Features.ReceiptItems.GetReceiptItems;
using ExpenseExplorer.Application.Features.Receipts.AddItem;
using ExpenseExplorer.Application.Features.Receipts.CreateHeader;
using ExpenseExplorer.Application.Features.Receipts.DeleteHeader;
using ExpenseExplorer.Application.Features.Receipts.DeleteItem;
using ExpenseExplorer.Application.Features.Receipts.Duplicate;
using ExpenseExplorer.Application.Features.Receipts.GetReceipt;
using ExpenseExplorer.Application.Features.Receipts.GetReceipts;
using ExpenseExplorer.Application.Features.Receipts.UpdateHeader;
using ExpenseExplorer.Application.Features.Receipts.UpdateItem;
using ExpenseExplorer.Application.Features.Stores.GetStores;
using ExpenseExplorer.Infrastructure.Database;
using ExpenseExplorer.Infrastructure.Database.TypeHandlers;
using ExpenseExplorer.Infrastructure.Features.Categories;
using ExpenseExplorer.Infrastructure.Features.Items;
using ExpenseExplorer.Infrastructure.Features.ReceiptItems;
using ExpenseExplorer.Infrastructure.Features.Receipts;
using ExpenseExplorer.Infrastructure.Features.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseExplorer.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<ReceiptRepository>()
            .AddScoped<ICreateReceiptHeaderPersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IUpdateReceiptHeaderPersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IDeleteReceiptHeaderPersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IDuplicateReceiptPersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IAddReceiptItemPersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IUpdateReceiptItemPersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IDeleteReceiptItemPersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IGetReceiptByIdPersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IGetReceiptSummariesPersistence>(sp => sp.GetRequiredService<ReceiptRepository>())
            .AddScoped<IGetStoresPersistence, StoreRepository>()
            .AddScoped<IGetItemsPersistence, ItemRepository>()
            .AddScoped<IGetCategoriesPersistence, CategoryRepository>()
            .AddScoped<IGetReceiptItemsPersistence, ReceiptItemsRepository>();
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