namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public sealed record GetReceiptQuery(string ReceiptId) : IQuery<Result<Receipt>>;
