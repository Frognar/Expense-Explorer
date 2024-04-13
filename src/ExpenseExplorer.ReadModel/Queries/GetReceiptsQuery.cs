namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public record GetReceiptsQuery(int PageSize, int PageNumber)
  : IQuery<Either<Failure, PageOf<ReceiptHeaders>>>
{
  public const int DefaultPageSize = 10;
  public const int MaxPageSize = 50;

  public GetReceiptsQuery(int? pageSize, int? pageNumber)
    : this(pageSize ?? DefaultPageSize, pageNumber ?? 1)
  {
  }

  public int PageSize { get; } = PageSize switch
  {
    < 1 => DefaultPageSize,
    > MaxPageSize => MaxPageSize,
    _ => PageSize,
  };

  public int PageNumber { get; } = Math.Max(1, PageNumber);
}
