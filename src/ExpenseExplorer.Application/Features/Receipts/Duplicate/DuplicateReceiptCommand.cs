using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.Duplicate;

internal sealed record DuplicateReceiptCommand(ReceiptId ReceiptId, PurchaseDate PurchaseDate)
    : ICommand<DuplicateReceiptResponse>;

internal static class DuplicateReceiptCommandFactory
{
    public static Func<ReceiptId, PurchaseDate, DuplicateReceiptCommand> Create => (id, p) =>
        new DuplicateReceiptCommand(id, p);
}