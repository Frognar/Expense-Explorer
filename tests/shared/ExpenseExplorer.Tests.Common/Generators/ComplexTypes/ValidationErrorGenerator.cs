using FunctionalCore.Failures;

namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public static class ValidationErrorGenerator
{
  public static Gen<ValidationError> Gen()
    =>
      from str in ArbMap.Default.GeneratorFor<NonEmptyString>()
      select ValidationError.Create("PROP." + str.Item, str.Item);

  public static Arbitrary<ValidationError> Arbitrary() => Gen().ToArbitrary();
}
