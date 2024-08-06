namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using DotResult;
using ExpenseExplorer.Domain.Receipts;

public sealed record RemovePurchaseCommand(string ReceiptId, string PurchaseId) : ICommand<Result<Receipt>>;
