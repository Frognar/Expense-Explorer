using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.Duplicate;

public record DuplicateReceiptCommand(ReceiptId ReceiptId, PurchaseDate PurchaseDate)
    : ICommand<DuplicateReceiptResponse>;

public static class DuplicateReceiptCommandFactory
{
    public static Func<ReceiptId, PurchaseDate, DuplicateReceiptCommand> Create => (id, p) =>
        new DuplicateReceiptCommand(id, p);
}