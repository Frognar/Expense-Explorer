namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public record GetReceiptQuery(string ReceiptId) : IQuery<Either<Failure, Receipt>>;
