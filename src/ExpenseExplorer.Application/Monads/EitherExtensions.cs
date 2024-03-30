namespace ExpenseExplorer.Application.Monads;

public static class EitherExtensions
{
  public static async Task<Either<L, R1>> FlatMapRightAsync<L, R, R1>(
    this Either<L, R> either,
    Func<R, Task<Either<L, R1>>> selector)
  {
    ArgumentNullException.ThrowIfNull(either);
    ArgumentNullException.ThrowIfNull(selector);
    return await either.Match(
      left => Task.FromResult(Left.From<L, R1>(left)),
      selector);
  }
}
