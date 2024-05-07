namespace FunctionalCore.Tests.Maybes;

using static HelperMethods;

public class LinqSelectTests
{
  [Property]
  public void MapsWithQuerySyntax(int value)
  {
    var result = GetMaybe(value);

    var projected =
      from v in result
      select v * 2;

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task MapsWithQuerySyntaxWhenSelectorIsAsync(int value)
  {
    var result = GetMaybe(value);

    var projected = await (
      from v in result
      select Task.FromResult(v * 2));

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task MapsWithQuerySyntaxWhenSourceIsAsync(int value)
  {
    var result = GetMaybe(value);

    var projected = await (
      from v in Task.FromResult(result)
      select v * 2);

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task MapsWithQuerySyntaxWhenBothSourceAndSelectorAreAsync(int value)
  {
    var result = GetMaybe(value);

    var projected = await (
      from v in Task.FromResult(result)
      select Task.FromResult(v * 2));

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }
}
