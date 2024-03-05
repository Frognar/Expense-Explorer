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
  public void ApplyWithTwoValidated(int v1, int v2) {
    Func<int, int, string> toString = (a, b) => (a + b).ToString();

    Validated<string> validatedResult =
      toString
        .Apply(Validate(v1))
        .Apply(Validate(v2));

    validatedResult
      .Match(AggregateErrors, v => v.ToString())
      .Should()
      .Be(GetExpectedString(v1, v2));
  }

  [Property]
  public void ApplyWithThreeValidated(int v1, int v2, int v3) {
    Func<int, int, int, string> toString = (a, b, c) => (a + b + c).ToString();

    Validated<string> validatedResult =
      toString
        .Apply(Validate(v1))
        .Apply(Validate(v2))
        .Apply(Validate(v3));

    validatedResult
      .Match(AggregateErrors, v => v.ToString())
      .Should()
      .Be(GetExpectedString(v1, v2, v3));
  }

  [Property]
  public void ApplyWithFourValidated(int v1, int v2, int v3, int v4) {
    Func<int, int, int, int, string> toString = (a, b, c, d) => (a + b + c + d).ToString();

    Validated<string> validatedResult =
      toString
        .Apply(Validate(v1))
        .Apply(Validate(v2))
        .Apply(Validate(v3))
        .Apply(Validate(v4));

    validatedResult
      .Match(AggregateErrors, v => v.ToString())
      .Should()
      .Be(GetExpectedString(v1, v2, v3, v4));
  }

  [Property]
  public void ApplyWithFiveValidated(int v1, int v2, int v3, int v4, int v5) {
    Func<int, int, int, int, int, string> toString = (a, b, c, d, e) => (a + b + c + d + e).ToString();

    Validated<string> validatedResult =
      toString
        .Apply(Validate(v1))
        .Apply(Validate(v2))
        .Apply(Validate(v3))
        .Apply(Validate(v4))
        .Apply(Validate(v5));

    validatedResult
      .Match(AggregateErrors, v => v.ToString())
      .Should()
      .Be(GetExpectedString(v1, v2, v3, v4, v5));
  }

  [Property]
  public void ApplyWithSixValidated(int v1, int v2, int v3, int v4, int v5, int v6) {
    Func<int, int, int, int, int, int, string> toString = (a, b, c, d, e, f) => (a + b + c + d + e + f).ToString();

    Validated<string> validatedResult =
      toString
        .Apply(Validate(v1))
        .Apply(Validate(v2))
        .Apply(Validate(v3))
        .Apply(Validate(v4))
        .Apply(Validate(v5))
        .Apply(Validate(v6));

    validatedResult
      .Match(AggregateErrors, v => v.ToString())
      .Should()
      .Be(GetExpectedString(v1, v2, v3, v4, v5, v6));
  }

  [Property]
  public void ApplyWithSevenValidated(int v1, int v2, int v3, int v4, int v5, int v6, int v7) {
    Func<int, int, int, int, int, int, int, string> toString = (a, b, c, d, e, f, g)
      => (a + b + c + d + e + f + g).ToString();

    Validated<string> validatedResult =
      toString
        .Apply(Validate(v1))
        .Apply(Validate(v2))
        .Apply(Validate(v3))
        .Apply(Validate(v4))
        .Apply(Validate(v5))
        .Apply(Validate(v6))
        .Apply(Validate(v7));

    validatedResult
      .Match(AggregateErrors, v => v.ToString())
      .Should()
      .Be(GetExpectedString(v1, v2, v3, v4, v5, v6, v7));
  }

  [Property]
  public void ApplyWithEightValidated(int v1, int v2, int v3, int v4, int v5, int v6, int v7, int v8) {
    Func<int, int, int, int, int, int, int, int, string> toString = (a, b, c, d, e, f, g, h)
      => (a + b + c + d + e + f + g + h).ToString();

    Validated<string> validatedResult =
      toString
        .Apply(Validate(v1))
        .Apply(Validate(v2))
        .Apply(Validate(v3))
        .Apply(Validate(v4))
        .Apply(Validate(v5))
        .Apply(Validate(v6))
        .Apply(Validate(v7))
        .Apply(Validate(v8));

    validatedResult
      .Match(AggregateErrors, v => v.ToString())
      .Should()
      .Be(GetExpectedString(v1, v2, v3, v4, v5, v6, v7, v8));
  }

  [Property]
  public void ApplyWithNineValidated(int v1, int v2, int v3, int v4, int v5, int v6, int v7, int v8, int v9) {
    Func<int, int, int, int, int, int, int, int, int, string> toString = (a, b, c, d, e, f, g, h, i)
      => (a + b + c + d + e + f + g + h + i).ToString();

    Validated<string> validatedResult =
      toString
        .Apply(Validate(v1))
        .Apply(Validate(v2))
        .Apply(Validate(v3))
        .Apply(Validate(v4))
        .Apply(Validate(v5))
        .Apply(Validate(v6))
        .Apply(Validate(v7))
        .Apply(Validate(v8))
        .Apply(Validate(v9));

    validatedResult
      .Match(AggregateErrors, v => v.ToString())
      .Should()
      .Be(GetExpectedString(v1, v2, v3, v4, v5, v6, v7, v8, v9));
  }

  [Property]
  public void ApplyWithTenValidated(int v1, int v2, int v3, int v4, int v5, int v6, int v7, int v8, int v9, int v0) {
    Func<int, int, int, int, int, int, int, int, int, int, string> toString = (a, b, c, d, e, f, g, h, i, j)
      => (a + b + c + d + e + f + g + h + i + j).ToString();

    Validated<string> validatedResult =
      toString
        .Apply(Validate(v1))
        .Apply(Validate(v2))
        .Apply(Validate(v3))
        .Apply(Validate(v4))
        .Apply(Validate(v5))
        .Apply(Validate(v6))
        .Apply(Validate(v7))
        .Apply(Validate(v8))
        .Apply(Validate(v9))
        .Apply(Validate(v0));

    validatedResult
      .Match(AggregateErrors, v => v.ToString())
      .Should()
      .Be(GetExpectedString(v1, v2, v3, v4, v5, v6, v7, v8, v9, v0));
  }

  private static string GetExpectedString(int value, params int[] values) {
    IEnumerable<ValidationError> errors = CreateErrors(CountInvalid(value, values)).ToList();
    return errors.Any() ? AggregateErrors(errors) : Sum(value, values).ToString();
  }

  private static int CountInvalid(int value, params int[] values) {
    return values.Count(v => v < 0) + (value < 0 ? 1 : 0);
  }

  private static IEnumerable<ValidationError> CreateErrors(int count) {
    return Enumerable.Repeat(ValidationError.Create("value", "NEGATIVE_VALUE"), count);
  }

  private static int Sum(int value, params int[] values) {
    return values.Sum() + value;
  }

  private static string AggregateErrors(IEnumerable<ValidationError> errors)
    => string.Join(", ", errors.Select(e => $"{e.Property}: {e.ErrorCode}"));

  private static Validated<int> Validate(int value)
    => value < 0
      ? Validated<int>.Fail([new ValidationError("value", "NEGATIVE_VALUE")])
      : Validated<int>.Success(value);
}
