namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;

public record CreateReceiptCommand(string Id, string Store, DateOnly PurchaseDate) : ICommand<Unit>;
