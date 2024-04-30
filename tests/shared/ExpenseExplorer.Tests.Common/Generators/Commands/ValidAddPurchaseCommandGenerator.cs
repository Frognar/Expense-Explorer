namespace ExpenseExplorer.Tests.Common.Generators.Commands;

using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Decimals;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Strings;

public static class ValidAddPurchaseCommandGenerator
{
  public static Gen<AddPurchaseCommand> Gen()
    =>
      from item in NonEmptyStringGenerator.Gen()
      from category in NonEmptyStringGenerator.Gen()
      from quantity in PositiveDecimalGenerator.Gen()
      from unitPrice in NonNegativeDecimalGenerator.Gen()
      from totalDiscount in ArbMap.Default.GeneratorFor<decimal?>()
      where !totalDiscount.HasValue || totalDiscount.Value >= 0
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
