namespace ExpenseExplorer.Tests.Common.Generators.Commands;

public static class InvalidAddPurchaseCommandGenerator
{
  public static Gen<AddPurchaseCommand> Gen()
  {
    Gen<AddPurchaseCommand> invalidItemName =
      from item in EmptyOrWhiteSpaceStringGenerator.Gen()
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

    Gen<AddPurchaseCommand> invalidCategory =
      from item in NonEmptyStringGenerator.Gen()
      from category in EmptyOrWhiteSpaceStringGenerator.Gen()
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

    Gen<AddPurchaseCommand> invalidQuantity =
      from item in NonEmptyStringGenerator.Gen()
      from category in NonEmptyStringGenerator.Gen()
      from quantity in NonPositiveDecimalGenerator.Gen()
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

    Gen<AddPurchaseCommand> invalidUnitPrice =
      from item in NonEmptyStringGenerator.Gen()
      from category in NonEmptyStringGenerator.Gen()
      from quantity in PositiveDecimalGenerator.Gen()
      from unitPrice in NegativeDecimalGenerator.Gen()
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

    Gen<AddPurchaseCommand> invalidTotalDiscount =
      from item in NonEmptyStringGenerator.Gen()
      from category in NonEmptyStringGenerator.Gen()
      from quantity in PositiveDecimalGenerator.Gen()
      from unitPrice in NonNegativeDecimalGenerator.Gen()
      from totalDiscount in NegativeDecimalGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new AddPurchaseCommand(
        "receiptId",
        item,
        category,
        quantity,
        unitPrice,
        totalDiscount,
        description);

    return FsCheck.Fluent.Gen.OneOf(
      invalidItemName,
      invalidCategory,
      invalidQuantity,
      invalidUnitPrice,
      invalidTotalDiscount);
  }

  public static Arbitrary<AddPurchaseCommand> Arbitrary() => Gen().ToArbitrary();
}
