using CommandHub.Commands;
using FunctionalCore;

namespace ExpenseExplorer.ReadModel.Commands;

public sealed record ChangePurchaseDateCommand(string ReceiptId, DateOnly PurchaseDate) : ICommand<Unit>;
