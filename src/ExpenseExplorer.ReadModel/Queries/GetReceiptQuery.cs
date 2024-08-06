namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Models;

public sealed record GetReceiptQuery(string ReceiptId) : IQuery<Result<Receipt>>;
