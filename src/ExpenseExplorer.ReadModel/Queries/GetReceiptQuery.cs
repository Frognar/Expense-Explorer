using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Models;

namespace ExpenseExplorer.ReadModel.Queries;

public sealed record GetReceiptQuery(string ReceiptId) : IQuery<Result<Receipt>>;
