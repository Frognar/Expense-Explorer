namespace ExpenseExplorer.Tests.Common.Generators.Commands;

public static class ValidAddPurchaseCommandGenerator
{
  public static Gen<AddPurchaseCommand> Gen()
    =>
      from item in NonEmptyStringGenerator.Gen()
      from category in NonEmptyStringGenerator.Gen()
      from quantity in PositiveDecimalGenerator.Gen()
      from unitPrice in NonNegativeDecimalGenerator.Gen()
      from totalDiscount in NullableNonNegativeDecimalGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new AddPurchaseCommand(
        "receiptId",
        item,
        category,
        quantity,
        unitPrice,
        totalDiscount,
        description);

  public static Arbitrary<AddPurchaseCommand> Arbitrary() => Gen().ToArbitrary();
}
