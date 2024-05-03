namespace ExpenseExplorer.Tests.Common.Generators.Commands;

public static class InvalidUpdatePurchaseDetailsCommandGenerator
{
  private const string _rId = "receiptWithPurchaseId";
  private const string _pId = "purchaseId";

  public static Gen<UpdatePurchaseDetailsCommand> Gen()
  {
    Gen<UpdatePurchaseDetailsCommand> invalidItem =
      from item in EmptyOrWhiteSpaceStringGenerator.Gen()
      from category in NullableNonEmptyStringGenerator.Gen()
      from quantity in NullablePositiveDecimalGenerator.Gen()
      from unitPrice in NullableNonNegativeDecimalGenerator.Gen()
      from discount in NullableNonNegativeDecimalGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new UpdatePurchaseDetailsCommand(_rId, _pId, item, category, quantity, unitPrice, discount, description);

    Gen<UpdatePurchaseDetailsCommand> invalidCategory =
      from item in NullableNonEmptyStringGenerator.Gen()
      from category in EmptyOrWhiteSpaceStringGenerator.Gen()
      from quantity in NullablePositiveDecimalGenerator.Gen()
      from unitPrice in NullableNonNegativeDecimalGenerator.Gen()
      from discount in NullableNonNegativeDecimalGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new UpdatePurchaseDetailsCommand(_rId, _pId, item, category, quantity, unitPrice, discount, description);

    Gen<UpdatePurchaseDetailsCommand> invalidQuantity =
      from item in NullableNonEmptyStringGenerator.Gen()
      from category in NullableNonEmptyStringGenerator.Gen()
      from quantity in NonPositiveDecimalGenerator.Gen()
      from unitPrice in NullableNonNegativeDecimalGenerator.Gen()
      from discount in NullableNonNegativeDecimalGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new UpdatePurchaseDetailsCommand(_rId, _pId, item, category, quantity, unitPrice, discount, description);

    Gen<UpdatePurchaseDetailsCommand> invalidUnitPrice =
      from item in NullableNonEmptyStringGenerator.Gen()
      from category in NullableNonEmptyStringGenerator.Gen()
      from quantity in NullablePositiveDecimalGenerator.Gen()
      from unitPrice in NegativeDecimalGenerator.Gen()
      from discount in NullableNonNegativeDecimalGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new UpdatePurchaseDetailsCommand(_rId, _pId, item, category, quantity, unitPrice, discount, description);

    Gen<UpdatePurchaseDetailsCommand> invalidDiscount =
      from item in NullableNonEmptyStringGenerator.Gen()
      from category in NullableNonEmptyStringGenerator.Gen()
      from quantity in NullablePositiveDecimalGenerator.Gen()
      from unitPrice in NullableNonNegativeDecimalGenerator.Gen()
      from discount in NegativeDecimalGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new UpdatePurchaseDetailsCommand(_rId, _pId, item, category, quantity, unitPrice, discount, description);

    return FsCheck.Fluent.Gen.OneOf(invalidItem, invalidCategory, invalidQuantity, invalidUnitPrice, invalidDiscount);
  }

  public static Arbitrary<UpdatePurchaseDetailsCommand> Arbitrary() => Gen().ToArbitrary();
}
