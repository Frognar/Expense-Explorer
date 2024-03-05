using ExpenseExplorer.Application.Validations;

namespace ExpenseExplorer.Application.Tests;

public class ValidatedTests {
  [Property]
  public void CanCreateValidationResult(string value) {
    Validated<string> result = Validation.Succeeded(value);
    result.Should().NotBeNull();
  }

  [Property]
  public void IsValidWithNoErrors(string value) {
    Validated<string> result = Validation.Succeeded(value);
    result.IsValid.Should().BeTrue();
  }

  [Property(Arbitrary = [typeof(ValidationErrorGenerator)])]
  public void IsInvalidWithErrors(ValidationError error) {
    Validated<string> result = Validation.Failed<string>([error]);
    result.IsValid.Should().BeFalse();
  }
}
