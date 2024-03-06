using ExpenseExplorer.Application.Validations;

namespace ExpenseExplorer.Tests.Shared.Generators;

public class ValidationErrorGenerator {
  public static Arbitrary<ValidationError> ValidationErrorGen() {
    return ArbMap.Default.ArbFor<NonEmptyString>()
      .Generator
      .Select(CreateError)
      .ToArbitrary();

    ValidationError CreateError(NonEmptyString str) => ValidationError.Create("PROP." + str.Item, str.Item);
  }
}
