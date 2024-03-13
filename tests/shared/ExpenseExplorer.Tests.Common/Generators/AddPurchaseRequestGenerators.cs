namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public record AddPurchaseRequestGenerators(
  Gen<string> Item,
  Gen<string> Category,
  Gen<decimal> Quantity,
  Gen<decimal> UnitPrice,
  Gen<decimal?> TotalDiscount,
  Gen<string> Description)
{
  public static readonly AddPurchaseRequestGenerators Valid = new(
    NonEmptyStringGenerator.NonEmptyStringGen().Generator,
    NonEmptyStringGenerator.NonEmptyStringGen().Generator,
    PositiveDecimalGenerator.PositiveDecimalGen().Generator,
    NonNegativeDecimalGenerator.NonNegativeDecimalGen().Generator,
    ArbMap.Default.ArbFor<decimal?>().Filter(d => !d.HasValue || d.Value >= 0).Generator,
    ArbMap.Default.ArbFor<string>().Generator);

  public Arbitrary<AddPurchaseRequest> Arbitrary
    => (
      from item in Item
      from category in Category
      from quantity in Quantity
      from unitPrice in UnitPrice
      from totalDiscount in TotalDiscount
      from description in Description
      select new AddPurchaseRequest(
        item,
        category,
        quantity,
        unitPrice,
        totalDiscount,
        description)).ToArbitrary();
}
