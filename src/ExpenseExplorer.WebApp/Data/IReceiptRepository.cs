using ExpenseExplorer.Application;
using ExpenseExplorer.WebApp.Models;

namespace ExpenseExplorer.WebApp.Data;

internal interface IReceiptRepository
{
    public Task<IEnumerable<string>> GetItemsAsync(string? search = null);
    public Task<IEnumerable<string>> GetCategoriesAsync(string? search = null);
    public Task AddAsync(ReceiptDetails receipt);
    public Task<ReceiptWithPurchases?> GetReceiptAsync(Guid id);
    public Task DeleteReceiptAsync(Guid id);

    public Task AddPurchaseAsync(Guid receiptId, PurchaseDetails purchase);
    public Task UpdatePurchaseAsync(Guid receiptId, PurchaseDetails purchase);
    public Task DeletePurchaseAsync(Guid receiptId, Guid purchaseId);
}