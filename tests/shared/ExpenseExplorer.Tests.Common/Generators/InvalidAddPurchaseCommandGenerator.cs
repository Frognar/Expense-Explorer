namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Application.Receipts.Commands;

public static class InvalidAddPurchaseCommandGenerator
{
  public static Arbitrary<AddPurchaseCommand> InvalidAddPurchaseCommandGen()
  {
    var invalidItemName =
      from item in EmptyStringGenerator.EmptyStringGen().Generator
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
        description);

    var invalidCategory =
      from item in NonEmptyStringGenerator.NonEmptyStringGen().Generator
      from category in EmptyStringGenerator.EmptyStringGen().Generator
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
        description);

    var invalidQuantity =
      from item in NonEmptyStringGenerator.NonEmptyStringGen().Generator
      from category in NonEmptyStringGenerator.NonEmptyStringGen().Generator
      from quantity in NonPositiveDecimalGenerator.NonPositiveDecimalGen().Generator
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
        description);

    var invalidUnitPrice =
      from item in NonEmptyStringGenerator.NonEmptyStringGen().Generator
      from category in NonEmptyStringGenerator.NonEmptyStringGen().Generator
      from quantity in PositiveDecimalGenerator.PositiveDecimalGen().Generator
      from unitPrice in NegativeDecimalGenerator.NegativeDecimalGen().Generator
      from totalDiscount in ArbMap.Default.ArbFor<decimal?>().Filter(d => !d.HasValue || d.Value >= 0).Generator
      from description in ArbMap.Default.ArbFor<string>().Generator
      select new AddPurchaseCommand(
        "receiptId",
        item,
        category,
        quantity,
        unitPrice,
        totalDiscount,
        description);

    var invalidTotalDiscount =
      from item in NonEmptyStringGenerator.NonEmptyStringGen().Generator
      from category in NonEmptyStringGenerator.NonEmptyStringGen().Generator
      from quantity in PositiveDecimalGenerator.PositiveDecimalGen().Generator
      from unitPrice in NonNegativeDecimalGenerator.NonNegativeDecimalGen().Generator
      from totalDiscount in NegativeDecimalGenerator.NegativeDecimalGen().Generator
      from description in ArbMap.Default.ArbFor<string>().Generator
      select new AddPurchaseCommand(
        "receiptId",
        item,
        category,
        quantity,
        unitPrice,
        totalDiscount,
        description);

    return Gen.OneOf(
        invalidItemName,
        invalidCategory,
        invalidQuantity,
        invalidUnitPrice,
        invalidTotalDiscount)
      .ToArbitrary();
  }
}
