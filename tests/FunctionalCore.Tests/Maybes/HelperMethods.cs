namespace FunctionalCore.Tests.Maybes;

public static class HelperMethods
{
  public static Maybe<int> GetMaybe(int value)
    => value < 0
      ? None.OfType<int>()
      : Some.From(value);

  public static void AssertMaybe(Maybe<int> maybe, bool expectNone, int expectedValue)
    => maybe.Match(() => 0, v => v)
      .Should()
      .Be(expectNone ? 0 : expectedValue);
}
