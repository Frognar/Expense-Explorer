namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public static class IdGenerator
{
  public static Gen<Maybe<Id>> GenMaybe()
    =>
      from id in FsCheck.Fluent.Gen.Constant(Id.Unique())
      select Some.From(id);

  public static Gen<Id> Gen()
    =>
      from maybe in GenMaybe()
      select maybe.ForceValue();

  public static Arbitrary<Id> Arbitrary() => Gen().ToArbitrary();
}
