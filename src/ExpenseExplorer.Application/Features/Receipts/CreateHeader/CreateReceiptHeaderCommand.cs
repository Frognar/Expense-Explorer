using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.CreateHeader;

public record CreateReceiptHeaderCommand(ReceiptId Id, Store Store, PurchaseDate PurchaseDate)
    : ICommand<CreateReceiptHeaderResponse>;

public static class CreateReceiptHeaderCommandFactory
{
    public static Func<Store, PurchaseDate, CreateReceiptHeaderCommand> Create => (s, p) =>
        new CreateReceiptHeaderCommand(ReceiptId.Unique(), s, p);
}