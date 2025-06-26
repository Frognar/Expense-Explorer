using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.UpdateHeader;

public sealed record UpdateReceiptHeaderCommand(ReceiptId ReceiptId, Store Store, PurchaseDate PurchaseDate)
    : ICommand<UpdateReceiptHeaderResponse>;

public static class UpdateReceiptHeaderCommandFactory
{
    public static Func<ReceiptId, Store, PurchaseDate, UpdateReceiptHeaderCommand> Create => (rId, s, p) =>
        new UpdateReceiptHeaderCommand(rId, s, p);
}