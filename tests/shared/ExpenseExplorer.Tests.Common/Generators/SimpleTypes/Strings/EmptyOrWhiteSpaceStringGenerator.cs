namespace ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Strings;

public static class EmptyOrWhiteSpaceStringGenerator
{
  public static Gen<string> Gen()
    => FsCheck.Fluent.Gen.OneOf(
      FsCheck.Fluent.Gen.Constant(string.Empty),
      WhiteSpaceStringGenerator.Gen());

  public static Arbitrary<string> Arbitrary() => Gen().ToArbitrary();
}
