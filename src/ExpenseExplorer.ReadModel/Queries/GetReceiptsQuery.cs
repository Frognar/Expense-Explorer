namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public record GetReceiptsQuery(int PageSize)
  : IQuery<Either<Failure, PageOf<ReceiptHeaders>>>
{
  public const int DefaultPageSize = 10;
  public const int MaxPageSize = 50;

  public GetReceiptsQuery(int? pageSize)
    : this(pageSize ?? DefaultPageSize)
  {
  }

  public int PageSize { get; } = PageSize switch
  {
    < 1 => DefaultPageSize,
    > MaxPageSize => MaxPageSize,
    _ => PageSize,
  };
}
