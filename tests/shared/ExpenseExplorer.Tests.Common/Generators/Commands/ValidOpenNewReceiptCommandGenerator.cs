namespace ExpenseExplorer.Tests.Common.Generators.Commands;

public static class ValidOpenNewReceiptCommandGenerator
{
  public static Gen<OpenNewReceiptCommand> Gen()
    =>
      from storeName in NonEmptyStringGenerator.Gen()
      from purchaseDate in NonFutureDateOnlyGenerator.Gen()
      select new OpenNewReceiptCommand(storeName, purchaseDate, Today);

  public static Arbitrary<OpenNewReceiptCommand> Arbitrary() => Gen().ToArbitrary();
}
