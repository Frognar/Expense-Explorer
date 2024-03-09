namespace ExpenseExplorer.Tests.Shared.Generators;

using API.Contract;

public class ValidAddPurchaseRequestGenerator
{
  public static Arbitrary<AddPurchaseRequest> ValidAddPurchaseRequestGen()
  {
    Func<decimal, bool> nonNegative = d => d >= 0;
    Func<decimal?, bool> nullOrNonNegative = d => d.HasValue == false || nonNegative(d.Value);
    Func<string?, bool> nullOrNonEmpty = s => s == null || s.Length > 0;
    return (
      from productName in NonEmptyStringGenerator.NonEmptyStringGen().Generator
      from productCategory in NonEmptyStringGenerator.NonEmptyStringGen().Generator
      from quantity in PositiveDecimalGenerator.PositiveDecimalGen().Generator
      from unitPrice in ArbMap.Default.ArbFor<decimal>().Filter(nonNegative).Generator
      from totalDiscount in ArbMap.Default.ArbFor<decimal?>().Filter(nullOrNonNegative).Generator
      from description in ArbMap.Default.ArbFor<string?>().Filter(nullOrNonEmpty).Generator
      select new AddPurchaseRequest(
        productName,
        productCategory,
        quantity,
        unitPrice,
        totalDiscount,
        description)).ToArbitrary();
  }
}
