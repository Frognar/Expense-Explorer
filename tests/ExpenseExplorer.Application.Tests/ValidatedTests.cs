using System.Diagnostics;
using ExpenseExplorer.Application.Validations;

namespace ExpenseExplorer.Application.Tests;

public class ValidatedTests {
  [Property]
  public void IsValidWithNoErrors(string value) {
    Validated<string> validated = Validation.Succeeded(value);
    validated.IsValid.Should().BeTrue();
  }

  [Property(Arbitrary = [typeof(ValidationErrorGenerator)])]
  public void IsInvalidWithErrors(ValidationError error) {
    Validated<string> validated = Validation.Failed<string>([error]);
    validated.IsValid.Should().BeFalse();
  }

  [Property]
  public void MatchValidated(int value) {
    Validated<int> validated = Validate(value);

    validated.Match(
        AggregateErrors,
        v => v.ToString()
      )
      .Should()
      .Be(value < 0 ? "value: NEGATIVE_VALUE" : value.ToString());
  }

  [Property]
  public void ApplyWithOneValidated(int value) {
    Validated<int> validated = Validate(value);
    Func<int, string> toString = x => x.ToString();

    Validated<string> validatedResult =
      toString
        .Apply(validated);

    validatedResult.IsValid.Should().Be(value >= 0);
  }

  [Property]
  public void ApplyWithTwoValidated(int value1, int value2) {
    Func<int, int, string> toString = (a, b) => (a + b).ToString();

    Validated<string> validatedResult =
      toString
        .Apply(Validate(value1))
        .Apply(Validate(value2));

    validatedResult.IsValid.Should().Be(value1 >= 0 && value2 >= 0);
  }

  [Property(Arbitrary = [typeof(ValidationErrorGenerator)])]
  public void ConcatErrorsWhenApplyingTwoValidated(ValidationError error1, ValidationError error2) {
    Func<int, int, string> toString = (a, b) => (a + b).ToString();

    Validated<string> validatedResult =
      toString
        .Apply(Validation.Failed<int>([error1]))
        .Apply(Validation.Failed<int>([error2]));

    validatedResult.Match(AggregateErrors, _ => throw new UnreachableException())
      .Should()
      .Be(AggregateErrors([error1, error2]));
  }

  private static string AggregateErrors(IEnumerable<ValidationError> errors)
    => string.Join(", ", errors.Select(e => $"{e.Property}: {e.ErrorCode}"));

  private static Validated<int> Validate(int value)
    => value < 0
      ? Validated<int>.Fail([new ValidationError("value", "NEGATIVE_VALUE")])
      : Validated<int>.Success(value);
}
