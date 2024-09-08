using DotMaybe;
using DotResult;

namespace ExpenseExplorer.Domain.Extensions;

public static class MaybeExtensions
{
  public static Result<T> ToResult<T>(this Maybe<T> maybe, Func<Failure> onNone)
    => maybe.Match(() => Fail.OfType<T>(onNone()), Success.From);
}
