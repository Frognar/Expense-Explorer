namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;
using FunctionalCore.Monads;

public sealed record GetIncomesQuery(
  int PageSize,
  int PageNumber,
  string Search,
  DateOnly ReceivedAfter,
  DateOnly ReceivedBefore,
  decimal MinAmount,
  decimal MaxAmount)
  : IQuery<Result<PageOf<Income>>>
{
  public const int DefaultPageSize = 10;
  public const int MaxPageSize = 50;

  public GetIncomesQuery(
    int? pageSize,
    int? pageNumber,
    string? search,
    DateOnly? receivedAfter,
    DateOnly? receivedBefore,
    decimal? minAmount,
    decimal? maxAmount)
    : this(
      pageSize ?? DefaultPageSize,
      pageNumber ?? 1,
      search ?? string.Empty,
      receivedAfter ?? DateOnly.MinValue,
      receivedBefore ?? DateOnly.MaxValue,
      minAmount ?? decimal.MinValue,
      maxAmount ?? decimal.MaxValue)
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
