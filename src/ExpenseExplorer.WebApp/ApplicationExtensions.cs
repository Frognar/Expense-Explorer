using ExpenseExplorer.Application.Features.Categories;
using ExpenseExplorer.Application.Features.Categories.GetCategories;
using ExpenseExplorer.Application.Features.Items;
using ExpenseExplorer.Application.Features.Items.GetItems;
using ExpenseExplorer.Application.Features.Receipts;
using ExpenseExplorer.Application.Features.Receipts.AddItem;
using ExpenseExplorer.Application.Features.Receipts.CreateHeader;
using ExpenseExplorer.Application.Features.Receipts.DeleteHeader;
using ExpenseExplorer.Application.Features.Receipts.DeleteItem;
using ExpenseExplorer.Application.Features.Receipts.Duplicate;
using ExpenseExplorer.Application.Features.Receipts.GetReceipt;
using ExpenseExplorer.Application.Features.Receipts.GetReceipts;
using ExpenseExplorer.Application.Features.Receipts.UpdateHeader;
using ExpenseExplorer.Application.Features.Receipts.UpdateItem;
using ExpenseExplorer.Application.Features.Stores;
using ExpenseExplorer.Application.Features.Stores.GetStores;

namespace ExpenseExplorer.WebApp;

internal static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
        => services
            .AddScoped(sp => ReceiptsFeaturesFactory.CreateCreateReceiptHeaderHandler(
                sp.GetRequiredService<ICreateReceiptHeaderPersistence>()))
            .AddScoped(sp => ReceiptsFeaturesFactory.CreateUpdateReceiptHeaderHandler(
                sp.GetRequiredService<IUpdateReceiptHeaderPersistence>()))
            .AddScoped(sp => ReceiptsFeaturesFactory.CreateDeleteReceiptHeaderHandler(
                sp.GetRequiredService<IDeleteReceiptHeaderPersistence>()))
            .AddScoped(sp => ReceiptsFeaturesFactory.CreateDuplicateReceiptHandler(
                sp.GetRequiredService<IDuplicateReceiptPersistence>()))
            .AddScoped(sp => ReceiptsFeaturesFactory.CreateAddReceiptItemHandler(
                sp.GetRequiredService<IAddReceiptItemPersistence>()))
            .AddScoped(sp => ReceiptsFeaturesFactory.CreateUpdateReceiptItemHandler(
                sp.GetRequiredService<IUpdateReceiptItemPersistence>()))
            .AddScoped(sp => ReceiptsFeaturesFactory.CreateDeleteReceiptItemHandler(
                sp.GetRequiredService<IDeleteReceiptItemPersistence>()))
            .AddScoped(sp => ReceiptsFeaturesFactory.CreateGetReceiptByIdHandler(
                sp.GetRequiredService<IGetReceiptByIdPersistence>()))
            .AddScoped(sp => ReceiptsFeaturesFactory.CreateGetReceiptSummariesHandler(
                sp.GetRequiredService<IGetReceiptSummariesPersistence>()))
            .AddScoped(sp => StoresFeatureFactory.CreateGetStoresCommandHandler(
                sp.GetRequiredService<IGetStoresPersistence>()))
            .AddScoped(sp => ItemsFeatureFactory.CreateGetItemsCommandHandler(
                sp.GetRequiredService<IGetItemsPersistence>()))
            .AddScoped(sp => CategoriesFeatureFactory.CreateGetCategoriesCommandHandler(
                sp.GetRequiredService<IGetCategoriesPersistence>()));
}