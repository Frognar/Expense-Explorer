using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.DeleteHeader;

public sealed record DeleteReceiptHeaderCommand(ReceiptId ReceiptId) : ICommand<DeleteReceiptHeaderResponse>;

public static class DeleteReceiptHeaderCommandFactory
{
    public static Func<ReceiptId, DeleteReceiptHeaderCommand> Create => rId =>
        new DeleteReceiptHeaderCommand(rId);
}