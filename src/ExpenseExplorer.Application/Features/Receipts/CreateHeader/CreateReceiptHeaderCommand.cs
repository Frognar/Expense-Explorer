using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.CreateHeader;

internal sealed record CreateReceiptHeaderCommand(ReceiptId Id, Store Store, PurchaseDate PurchaseDate)
    : ICommand<CreateReceiptHeaderResponse>;

internal static class CreateReceiptHeaderCommandFactory
{
    public static Func<Store, PurchaseDate, CreateReceiptHeaderCommand> Create => (s, p) =>
        new CreateReceiptHeaderCommand(ReceiptId.Unique(), s, p);
}