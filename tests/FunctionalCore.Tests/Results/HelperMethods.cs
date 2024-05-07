namespace FunctionalCore.Tests.Results;

public static class HelperMethods
{
  public static Result<int> GetResult(int value)
    => value < 0
      ? Fail.OfType<int>(Failure.Validation("value", "Negative"))
      : Success.From(value);

  public static void AssertResult(Result<int> result, bool expectFailure, int expectedValue)
    => result.Match(_ => 0, v => v)
      .Should()
      .Be(expectFailure ? 0 : expectedValue);
}
