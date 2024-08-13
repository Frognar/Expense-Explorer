using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Models;

namespace ExpenseExplorer.ReadModel.Queries;

public record GetSourcesQuery(
  int PageSize,
  int PageNumber,
  string Search)
  : IQuery<Result<PageOf<Source>>>
{
  public const int DefaultPageSize = 25;
  public const int MaxPageSize = 100;

  public GetSourcesQuery(int? pageSize, int? pageNumber, string? search)
    : this(
      pageSize ?? DefaultPageSize,
      pageNumber ?? 1,
      search ?? string.Empty)
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
