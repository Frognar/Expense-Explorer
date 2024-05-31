namespace ExpenseExplorer.GUI.Helpers;

using Radzen;

public static class LoadDataArgsExtensions
{
  public static (int PageNumber, int PageSize) GetPagingParameters(this LoadDataArgs args)
  {
    return args switch
    {
      { Skip: not null, Top: not null } => ((args.Skip.Value / args.Top.Value) + 1, args.Top.Value),
      { Top: not null } => (1, args.Top.Value),
      _ => (1, 10),
    };
  }

  public static (string? SortBy, string? SortOrder) GetOrderByParameters(this LoadDataArgs args)
  {
    ArgumentNullException.ThrowIfNull(args);
    if (string.IsNullOrWhiteSpace(args.OrderBy))
    {
      return (null, null);
    }

    return args.OrderBy.Split(' ') is { Length: 2 } sort
      ? (sort[0], sort[1])
      : (null, null);
  }
}
