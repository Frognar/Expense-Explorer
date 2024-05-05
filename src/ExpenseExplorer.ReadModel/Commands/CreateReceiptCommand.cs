namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using FunctionalCore;

public sealed record CreateReceiptCommand(string Id, string Store, DateOnly PurchaseDate) : ICommand<Unit>;
