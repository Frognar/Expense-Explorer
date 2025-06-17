using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.DeleteItem;

public sealed record DeleteReceiptItemCommand(ReceiptId ReceiptId, ReceiptItemId ReceiptItemId)
    : ICommand<DeleteReceiptItemResponse>;

public static class DeleteReceiptItemCommandFactory
{
    public static Func<ReceiptId, ReceiptItemId, DeleteReceiptItemCommand> Create => (r, i) =>
        new DeleteReceiptItemCommand(r, i);
}