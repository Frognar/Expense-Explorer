namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public static class IdGenerator
{
  public static Gen<Id> Gen() => FsCheck.Fluent.Gen.Constant(Id.Unique());

  public static Arbitrary<Id> Arbitrary() => Gen().ToArbitrary();
}
