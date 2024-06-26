namespace ExpenseExplorer.Tests.Common;

public static class TestMaybeExtensions
{
  public static T ForceValue<T>(this Maybe<T> maybe)
    => maybe.Match(() => throw new InvalidOperationException(), v => v);
}
