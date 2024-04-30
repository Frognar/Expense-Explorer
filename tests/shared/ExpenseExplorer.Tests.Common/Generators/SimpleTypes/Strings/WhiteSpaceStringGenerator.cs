namespace ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Strings;

public static class WhiteSpaceStringGenerator
{
  private static readonly Gen<char> _whitespaceCharGen = FsCheck.Fluent.Gen.OneOf(
    FsCheck.Fluent.Gen.Constant(' '),
    FsCheck.Fluent.Gen.Constant('\t'),
    FsCheck.Fluent.Gen.Constant('\n'),
    FsCheck.Fluent.Gen.Constant('\r'));

  public static Gen<string> Gen()
    =>
      from length in FsCheck.FSharp.Gen.Choose(1, 10)
      from chars in FsCheck.FSharp.Gen.ArrayOf(length, _whitespaceCharGen)
      select new string(chars);

  public static Arbitrary<string> Arbitrary() => Gen().ToArbitrary();
}
