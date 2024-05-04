namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Domain.Receipts;
using FunctionalCore.Monads;

public sealed record RemovePurchaseCommand(string ReceiptId, string PurchaseId) : ICommand<Result<Receipt>>;
