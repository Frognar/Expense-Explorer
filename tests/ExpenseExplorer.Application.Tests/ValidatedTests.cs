namespace ExpenseExplorer.Application.Tests;

using System.Globalization;
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
      .Be(value < 0 ? "value: NEGATIVE_VALUE" : ToInvariantString(value));
  }

  private static string AggregateErrors(IEnumerable<ValidationError> errors)
    => string.Join(", ", errors.Select(e => $"{e.Property}: {e.ErrorCode}"));

  private static Validated<int> Validate(int value)
    => value < 0
      ? Validated<int>.Fail([new ValidationError("value", "NEGATIVE_VALUE")])
      : Validated<int>.Success(value);

  private static string ToInvariantString(int value) => value.ToString(CultureInfo.InvariantCulture);
}
