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
    Validated<int> validated = value < 0
      ? Validated<int>.Fail([new ValidationError("value", "NEGATIVE_VALUE")])
      : Validated<int>.Success(value);

    validated.Match(
        errors => string.Join(", ", errors.Select(e => $"{e.Property}: {e.ErrorCode}")),
        v => v.ToString()
      )
      .Should()
      .Be(value < 0 ? "value: NEGATIVE_VALUE" : value.ToString());
  }

  [Property]
  public void ApplyWithOneValidated(int value) {
    Validated<int> validated = value < 0
      ? Validated<int>.Fail([new ValidationError("value", "NEGATIVE_VALUE")])
      : Validated<int>.Success(value);

    Func<int, string> toString = x => x.ToString();

    Validated<string> validatedResult =
      toString
        .Apply(validated);

    validatedResult.IsValid.Should().Be(value >= 0);
  }
}
