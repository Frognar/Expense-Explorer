namespace ExpenseExplorer.Tests.Common.Generators.Commands;

public static class ValidUpdatePurchaseDetailsCommandGenerator
{
  public static Gen<UpdatePurchaseDetailsCommand> Gen()
    =>
      from item in NullableNonEmptyStringGenerator.Gen()
      from category in NullableNonEmptyStringGenerator.Gen()
      from quantity in NullablePositiveDecimalGenerator.Gen()
      from unitPrice in NullableNonNegativeDecimalGenerator.Gen()
      from totalDiscount in NullableNonNegativeDecimalGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new UpdatePurchaseDetailsCommand(
        "receiptWithPurchaseId",
        "purchaseId",
        item,
        category,
        quantity,
        unitPrice,
        totalDiscount,
        description);

  public static Arbitrary<UpdatePurchaseDetailsCommand> Arbitrary() => Gen().ToArbitrary();
}
