namespace ExpenseExplorer.Application.Tests;

using System.Globalization;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Validations;

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
    Validated<string> validated = Validation.Failed<string>(ValidationFailure.SingleFailure(error));
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
  public void ChangeToEither(int value)
  {
    Validated<int> validated = Validate(value);

    Either<Failure, int> either = validated.ToEither();

    either.Match(AggregateErrors, v => v.ToString(CultureInfo.InvariantCulture))
      .Should()
      .Be(GetExpectedString(value));
  }

  private static string GetExpectedString(int value) => value < 0 ? "value: NEGATIVE_VALUE" : ToInvariantString(value);

  private static string AggregateErrors(Failure errors) => AggregateErrors((ValidationFailure)errors);

  private static string AggregateErrors(ValidationFailure errors)
    => string.Join(", ", errors.Errors.Select(e => $"{e.Property}: {e.ErrorCode}"));

  private static Validated<int> Validate(int value)
    => value < 0
      ? Validated<int>.Fail(ValidationFailure.SingleFailure("value", "NEGATIVE_VALUE"))
      : Validated<int>.Success(value);

  private static string ToInvariantString(int value) => value.ToString(CultureInfo.InvariantCulture);
}
