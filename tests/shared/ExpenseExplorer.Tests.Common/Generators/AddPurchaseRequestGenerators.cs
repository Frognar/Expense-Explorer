namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public record AddPurchaseRequestGenerators(
  Gen<string> ProductName,
  Gen<string> ProductCategory,
  Gen<decimal> Quantity,
  Gen<decimal> UnitPrice,
  Gen<decimal?> TotalDiscount,
  Gen<string?> Description)
{
  public static readonly AddPurchaseRequestGenerators Valid = new(
    NonEmptyStringGenerator.NonEmptyStringGen().Generator,
    NonEmptyStringGenerator.NonEmptyStringGen().Generator,
    PositiveDecimalGenerator.PositiveDecimalGen().Generator,
    ArbMap.Default.ArbFor<decimal>().Filter(d => d >= 0).Generator,
    ArbMap.Default.ArbFor<decimal?>().Filter(d => !d.HasValue || d.Value >= 0).Generator,
    ArbMap.Default.ArbFor<string?>().Filter(s => s == null || s.Length > 0).Generator);

  public Arbitrary<AddPurchaseRequest> Arbitrary
    => (
      from productName in ProductName
      from productCategory in ProductCategory
      from quantity in Quantity
      from unitPrice in UnitPrice
      from totalDiscount in TotalDiscount
      from description in Description
      select new AddPurchaseRequest(
        productName,
        productCategory,
        quantity,
        unitPrice,
        totalDiscount,
        description)).ToArbitrary();
}
