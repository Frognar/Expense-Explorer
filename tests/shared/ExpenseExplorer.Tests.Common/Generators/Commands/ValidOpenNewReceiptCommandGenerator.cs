namespace ExpenseExplorer.Tests.Common.Generators.Commands;

public static class ValidOpenNewReceiptCommandGenerator
{
  public static Gen<OpenNewReceiptCommand> Gen()
    =>
      from storeName in NonEmptyStringGenerator.Gen()
      from purchaseDate in DateOnlyGenerator.Gen()
      select new OpenNewReceiptCommand(storeName, purchaseDate, purchaseDate);

  public static Arbitrary<OpenNewReceiptCommand> Arbitrary() => Gen().ToArbitrary();
}
