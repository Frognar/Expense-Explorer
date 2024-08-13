using CommandHub.Commands;
using DotResult;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Application.Receipts.Commands;

public sealed record RemovePurchaseCommand(string ReceiptId, string PurchaseId) : ICommand<Result<Receipt>>;
