namespace FunctionalCore.Tests.Results;

using static HelperMethods;

public class LinqSelectTests
{
  [Property]
  public void MapsWithQuerySyntax(int value)
  {
    var result = GetResult(value);

    var projected =
      from v in result
      select v * 2;

    AssertResult(projected, expectFailure: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task MapsWithQuerySyntaxWhenSelectorIsAsync(int value)
  {
    var result = GetResult(value);

    var projected = await (
      from v in result
      select Task.FromResult(v * 2));

    AssertResult(projected, expectFailure: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task MapsWithQuerySyntaxWhenSourceIsAsync(int value)
  {
    var result = GetResult(value);

    var projected = await (
      from v in Task.FromResult(result)
      select v * 2);

    AssertResult(projected, expectFailure: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task MapsWithQuerySyntaxWhenBothSourceAndSelectorAreAsync(int value)
  {
    var result = GetResult(value);

    var projected = await (
      from v in Task.FromResult(result)
      select Task.FromResult(v * 2));

    AssertResult(projected, expectFailure: value < 0, expectedValue: value * 2);
  }
}
