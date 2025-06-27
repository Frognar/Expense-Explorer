using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.UpdateHeader;

internal sealed record UpdateReceiptHeaderCommand(ReceiptId ReceiptId, Store Store, PurchaseDate PurchaseDate)
    : ICommand<UpdateReceiptHeaderResponse>;

internal static class UpdateReceiptHeaderCommandFactory
{
    public static Func<ReceiptId, Store, PurchaseDate, UpdateReceiptHeaderCommand> Create => (rId, s, p) =>
        new UpdateReceiptHeaderCommand(rId, s, p);
}