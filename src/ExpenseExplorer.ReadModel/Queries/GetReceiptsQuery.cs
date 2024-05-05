namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public sealed record GetReceiptsQuery(
  int PageSize,
  int PageNumber,
  string Search,
  DateOnly After,
  DateOnly Before,
  decimal MinTotal,
  decimal MaxTotal)
  : IQuery<Either<Failure, PageOf<ReceiptHeaders>>>
{
  public const int DefaultPageSize = 10;
  public const int MaxPageSize = 50;

  public GetReceiptsQuery(
    int? pageSize,
    int? pageNumber,
    string? search,
    DateOnly? after,
    DateOnly? before,
    decimal? minTotal,
    decimal? maxTotal)
    : this(
      pageSize ?? DefaultPageSize,
      pageNumber ?? 1,
      search ?? string.Empty,
      after ?? DateOnly.MinValue,
      before ?? DateOnly.MaxValue,
      minTotal ?? decimal.MinValue,
      maxTotal ?? decimal.MaxValue)
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
