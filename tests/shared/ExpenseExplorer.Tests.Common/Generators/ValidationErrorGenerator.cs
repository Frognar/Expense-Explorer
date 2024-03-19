namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Application.Errors;

public static class ValidationErrorGenerator
{
  public static Arbitrary<ValidationError> ValidationErrorGen()
  {
    return ArbMap.Default.ArbFor<NonEmptyString>()
      .Generator
      .Select(str => CreateError(str.Item))
      .ToArbitrary();

    ValidationError CreateError(string str) => ValidationError.Create("PROP." + str, str);
  }
}
