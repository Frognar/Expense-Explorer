namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using FunctionalCore;

public sealed record RemovePurchaseCommand(string ReceiptId, string PurchaseId) : ICommand<Unit>;
