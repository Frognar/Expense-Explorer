namespace FunctionalCore.Tests;

using System.Globalization;
using FunctionalCore.Failures;
using FunctionalCore.Validations;

public class ApplicativeValidatedTests
{
  [Property]
  public void ApplyWithOneValidated(int value)
  {
    Validated<int> validated = Validate(value);
    Func<int, string> toString = ToInvariantString;

    Validated<string> validatedResult =
      toString
        .Apply(validated);

    validatedResult.IsValid.Should().Be(value >= 0);
  }

  [Property]
  public void ApplyWithTwoValidated(int v1, int v2)
  {
    Func<int, int, string> toString = (a, b) => ToInvariantString(a + b);

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
  public void ApplyWithThreeValidated(int v1, int v2, int v3)
  {
    Func<int, int, int, string> toString = (a, b, c) => ToInvariantString(a + b + c);

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
  public void ApplyWithFourValidated(int v1, int v2, int v3, int v4)
  {
    Func<int, int, int, int, string> toString = (a, b, c, d) => ToInvariantString(a + b + c + d);

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
  public void ApplyWithFiveValidated(int v1, int v2, int v3, int v4, int v5)
  {
    Func<int, int, int, int, int, string> toString = (a, b, c, d, e) => ToInvariantString(a + b + c + d + e);

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
  public void ApplyWithSixValidated(int v1, int v2, int v3, int v4, int v5, int v6)
  {
    Func<int, int, int, int, int, int, string>
      toString = (a, b, c, d, e, f) => ToInvariantString(a + b + c + d + e + f);

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
  public void ApplyWithSevenValidated(int v1, int v2, int v3, int v4, int v5, int v6, int v7)
  {
    Func<int, int, int, int, int, int, int, string> toString = (a, b, c, d, e, f, g)
      => ToInvariantString(a + b + c + d + e + f + g);

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
  public void ApplyWithEightValidated(int v1, int v2, int v3, int v4, int v5, int v6, int v7, int v8)
  {
    Func<int, int, int, int, int, int, int, int, string> toString = (a, b, c, d, e, f, g, h)
      => ToInvariantString(a + b + c + d + e + f + g + h);

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
  public void ApplyWithNineValidated(int v1, int v2, int v3, int v4, int v5, int v6, int v7, int v8, int v9)
  {
    Func<int, int, int, int, int, int, int, int, int, string> toString = (a, b, c, d, e, f, g, h, i)
      => ToInvariantString(a + b + c + d + e + f + g + h + i);

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
  public void ApplyWithTenValidated(int v1, int v2, int v3, int v4, int v5, int v6, int v7, int v8, int v9, int v0)
  {
    Func<int, int, int, int, int, int, int, int, int, int, string> toString = (a, b, c, d, e, f, g, h, i, j)
      => ToInvariantString(a + b + c + d + e + f + g + h + i + j);

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

  private static string GetExpectedString(int value, params int[] values)
  {
    ValidationFailure errors = CreateErrors(CountInvalid(value, values));
    return errors.Errors.Any() ? AggregateErrors(errors) : ToInvariantString(Sum(value, values));
  }

  private static int CountInvalid(int value, params int[] values)
  {
    return values.Count(v => v < 0) + (value < 0 ? 1 : 0);
  }

  private static ValidationFailure CreateErrors(int count)
  {
    return new ValidationFailure(Enumerable.Repeat(ValidationError.Create("value", "NEGATIVE_VALUE"), count));
  }

  private static int Sum(int value, params int[] values)
  {
    return values.Sum() + value;
  }

  private static string AggregateErrors(ValidationFailure errors)
    => string.Join(", ", errors.Errors.Select(e => $"{e.Property}: {e.ErrorCode}"));

  private static Validated<int> Validate(int value)
    => value < 0
      ? Validation.Failed<int>(ValidationFailure.SingleFailure("value", "NEGATIVE_VALUE"))
      : Validation.Succeeded(value);

  private static string ToInvariantString(int value) => value.ToString(CultureInfo.InvariantCulture);
}
