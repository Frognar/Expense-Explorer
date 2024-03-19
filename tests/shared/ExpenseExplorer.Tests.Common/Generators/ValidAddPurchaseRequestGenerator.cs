namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public static class ValidAddPurchaseRequestGenerator
{
  public static Arbitrary<AddPurchaseRequest> ValidAddPurchaseRequestGen()
  {
    return (
      from item in NonEmptyStringGenerator.NonEmptyStringGen().Generator
      from category in NonEmptyStringGenerator.NonEmptyStringGen().Generator
      from quantity in PositiveDecimalGenerator.PositiveDecimalGen().Generator
      from unitPrice in NonNegativeDecimalGenerator.NonNegativeDecimalGen().Generator
      from totalDiscount in ArbMap.Default.ArbFor<decimal?>().Filter(d => !d.HasValue || d.Value >= 0).Generator
      from description in ArbMap.Default.ArbFor<string>().Generator
      select new AddPurchaseRequest(item, category, quantity, unitPrice, totalDiscount, description)).ToArbitrary();
  }
}
