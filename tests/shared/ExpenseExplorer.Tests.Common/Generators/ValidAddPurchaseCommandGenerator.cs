namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Application.Receipts.Commands;

public static class ValidAddPurchaseCommandGenerator
{
  public static Arbitrary<AddPurchaseCommand> ValidAddPurchaseCommandGen()
  {
    return (
        from item in NonEmptyStringGenerator.NonEmptyStringGen().Generator
        from category in NonEmptyStringGenerator.NonEmptyStringGen().Generator
        from quantity in PositiveDecimalGenerator.PositiveDecimalGen().Generator
        from unitPrice in NonNegativeDecimalGenerator.NonNegativeDecimalGen().Generator
        from totalDiscount in ArbMap.Default.ArbFor<decimal?>().Filter(d => !d.HasValue || d.Value >= 0).Generator
        from description in ArbMap.Default.ArbFor<string>().Generator
        select new AddPurchaseCommand(
          "receiptId",
          item,
          category,
          quantity,
          unitPrice,
          totalDiscount,
          description))
      .ToArbitrary();
  }
}
