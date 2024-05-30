namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;
using FunctionalCore.Monads;

public sealed record GetIncomesQuery(
  int PageSize,
  int PageNumber,
  string Source,
  string Category,
  string Description,
  DateOnly ReceivedAfter,
  DateOnly ReceivedBefore,
  decimal MinAmount,
  decimal MaxAmount,
  string SortBy,
  string SortOrder)
  : IQuery<Result<PageOf<Income>>>
{
  public const int DefaultPageSize = 10;
  public const int MaxPageSize = 50;

  public GetIncomesQuery(
    int? pageSize,
    int? pageNumber,
    string? source,
    string? category,
    string? description,
    DateOnly? receivedAfter,
    DateOnly? receivedBefore,
    decimal? minAmount,
    decimal? maxAmount,
    string? sortBy,
    string? sortOrder)
    : this(
      pageSize ?? DefaultPageSize,
      pageNumber ?? 1,
      source ?? string.Empty,
      category ?? string.Empty,
      description ?? string.Empty,
      receivedAfter ?? DateOnly.MinValue,
      receivedBefore ?? DateOnly.MaxValue,
      minAmount ?? decimal.MinValue,
      maxAmount ?? decimal.MaxValue,
      sortBy ?? string.Empty,
      sortOrder ?? string.Empty)
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
