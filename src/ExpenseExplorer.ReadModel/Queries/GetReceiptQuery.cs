namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public record GetReceiptQuery(int PageSize) : IQuery<Either<Failure, PageOf<ReceiptHeaders>>>;
