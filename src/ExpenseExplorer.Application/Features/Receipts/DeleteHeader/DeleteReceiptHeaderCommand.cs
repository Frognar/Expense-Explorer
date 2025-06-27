using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.DeleteHeader;

internal sealed record DeleteReceiptHeaderCommand(ReceiptId ReceiptId) : ICommand<DeleteReceiptHeaderResponse>;

internal static class DeleteReceiptHeaderCommandFactory
{
    public static Func<ReceiptId, DeleteReceiptHeaderCommand> Create => rId =>
        new DeleteReceiptHeaderCommand(rId);
}