namespace ExpenseExplorer.API.Mappers;

using ExpenseExplorer.API.Contract;
using ExpenseExplorer.Domain.Receipts;

public static class ReceiptMapper
{
  public static OpenNewReceiptResponse MapToResponse(this Receipt receipt)
  {
    ArgumentNullException.ThrowIfNull(receipt);
    return new OpenNewReceiptResponse(receipt.Id.Value, receipt.Store.Name, receipt.PurchaseDate.Date);
  }
}
