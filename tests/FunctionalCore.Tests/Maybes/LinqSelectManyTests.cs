namespace FunctionalCore.Tests.Maybes;

using static HelperMethods;

public class LinqSelectManyTests
{
  [Property]
  public void FlatMapsWithQuerySyntax(int value)
  {
    var result = GetMaybe(value);

    var projected =
      from v0 in result
      from v1 in result
      select v0 + v1;

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value + value);
  }

  [Property]
  public async Task FlatMapsWithQuerySyntaxWhenSelectorIsAsync(int value)
  {
    var result = GetMaybe(value);

    var projected = await (
      from v0 in result
      from v1 in Task.FromResult(result)
      select v0 + v1);

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value + value);
  }

  [Property]
  public async Task FlatMapsWithQuerySyntaxWhenProjectorIsAsync(int value)
  {
    var result = GetMaybe(value);

    var projected = await (
      from v0 in result
      from v1 in result
      select Task.FromResult(v0 + v1));

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value + value);
  }

  [Property]
  public async Task FlatMapsWithQuerySyntaxWhenBothSelectorAndProjectorAreAsync(int value)
  {
    var result = GetMaybe(value);

    var projected = await (
      from v0 in result
      from v1 in Task.FromResult(result)
      select Task.FromResult(v0 + v1));

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value + value);
  }

  [Property]
  public async Task FlatMapsWithQuerySyntaxWhenSourceIsAsync(int value)
  {
    var result = GetMaybe(value);

    var projected = await (
      from v0 in Task.FromResult(result)
      from v1 in result
      select v0 + v1);

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value + value);
  }

  [Property]
  public async Task FlatMapsWithQuerySyntaxWhenBothSourceAndSelectorAreAsync(int value)
  {
    var result = GetMaybe(value);

    var projected = await (
      from v0 in Task.FromResult(result)
      from v1 in Task.FromResult(result)
      select v0 + v1);

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value + value);
  }

  [Property]
  public async Task FlatMapsWithQuerySyntaxWhenBothSourceAndProjectorAreAsync(int value)
  {
    var result = GetMaybe(value);

    var projected = await (
      from v0 in Task.FromResult(result)
      from v1 in result
      select Task.FromResult(v0 + v1));

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value + value);
  }

  [Property]
  public async Task FlatMapsWithQuerySyntaxWhenAllSourceSelectorAndProjectorAreAsync(int value)
  {
    var result = GetMaybe(value);

    var projected = await (
      from v0 in Task.FromResult(result)
      from v1 in Task.FromResult(result)
      select Task.FromResult(v0 + v1));

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value + value);
  }
}
