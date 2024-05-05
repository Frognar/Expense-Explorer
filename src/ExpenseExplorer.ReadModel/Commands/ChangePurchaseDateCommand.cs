namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using FunctionalCore;

public sealed record ChangePurchaseDateCommand(string ReceiptId, DateOnly PurchaseDate) : ICommand<Unit>;
