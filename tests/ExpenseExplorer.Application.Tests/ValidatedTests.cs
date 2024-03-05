using ExpenseExplorer.Application.Validations;

namespace ExpenseExplorer.Application.Tests;

public class ValidatedTests {
  [Property]
  public void CanCreateValidationResult(string value) {
    Validated<string> result = new(value);
    result.Should().NotBeNull();
  }
}
