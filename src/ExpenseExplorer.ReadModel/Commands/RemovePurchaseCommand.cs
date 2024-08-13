using CommandHub.Commands;
using FunctionalCore;

namespace ExpenseExplorer.ReadModel.Commands;

public sealed record RemovePurchaseCommand(string ReceiptId, string PurchaseId) : ICommand<Unit>;
