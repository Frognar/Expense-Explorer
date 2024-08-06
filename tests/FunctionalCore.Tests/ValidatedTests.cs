namespace FunctionalCore.Tests;

using DotResult;
using ExpenseExplorer.Tests.Common;

public class ValidatedTests
{
  [Property]
  public void IsValidWithNoErrors(string value)
  {
    Validated<string> validated = Validation.Succeeded(value);
    validated.IsValid.Should().BeTrue();
  }

  [Property(Arbitrary = [typeof(ValidationErrorGenerator)])]
  public void IsInvalidWithErrors(ValidationError error)
  {
    Validated<string> validated = Validation.Failed<string>([error]);
    validated.IsValid.Should().BeFalse();
  }

  [Property]
  public void MatchValidated(int value)
  {
    Validated<int> validated = Validate(value);

    validated.Match(
        AggregateErrors,
        ToInvariantString)
      .Should()
      .Be(GetExpectedString(value));
  }

  [Property]
  public void MapValidated(int value)
  {
    Validated<int> validated = Validate(value);

    validated.Map(ToInvariantString)
      .Match(
        AggregateErrors,
        v => v)
      .Should()
      .Be(GetExpectedString(value));
  }

  [Property]
  public void MapValidatedWithQuerySyntax(int value)
  {
    Validated<int> validated = Validate(value);

    Validated<string> validatedResult =
      from v in validated
      select ToInvariantString(v);

    validatedResult
      .Match(
        AggregateErrors,
        v => v)
      .Should()
      .Be(GetExpectedString(value));
  }

  [Property]
  public void ChangeToResult(int value)
  {
    Validated<int> validated = Validate(value);

    Result<int> result = Validation.ToResult(validated);

    result.Match(AggregateErrors, v => v.ToString(CultureInfo.InvariantCulture))
      .Should()
      .Be(GetExpectedString(value));
  }

  private static string GetExpectedString(int value) => value < 0 ? "value: NEGATIVE_VALUE" : ToInvariantString(value);

  private static string AggregateErrors(Failure failure)
    => AggregateErrors(
      failure.Match(
        (_, _) => Enumerable.Empty<ValidationError>(),
        (_, _) => Enumerable.Empty<ValidationError>(),
        (_, errors) => errors));

  private static string AggregateErrors(IEnumerable<ValidationError> errors)
    => string.Join(", ", errors.Select(e => $"{e.Property}: {e.ErrorCode}"));

  private static Validated<int> Validate(int value)
    => value < 0
      ? Validation.Failed<int>([ValidationError.Create("value", "NEGATIVE_VALUE")])
      : Validation.Succeeded(value);

  private static string ToInvariantString(int value) => value.ToString(CultureInfo.InvariantCulture);
}
