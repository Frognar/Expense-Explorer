using CommandHub.Commands;
using FunctionalCore;

namespace ExpenseExplorer.ReadModel.Commands;

public sealed record CreateReceiptCommand(string Id, string Store, DateOnly PurchaseDate) : ICommand<Unit>;
