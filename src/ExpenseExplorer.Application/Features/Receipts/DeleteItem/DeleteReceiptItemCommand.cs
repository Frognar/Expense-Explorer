using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.DeleteItem;

internal sealed record DeleteReceiptItemCommand(ReceiptId ReceiptId, ReceiptItemId ReceiptItemId)
    : ICommand<DeleteReceiptItemResponse>;

internal static class DeleteReceiptItemCommandFactory
{
    public static Func<ReceiptId, ReceiptItemId, DeleteReceiptItemCommand> Create => (r, i) =>
        new DeleteReceiptItemCommand(r, i);
}