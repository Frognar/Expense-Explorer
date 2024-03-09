namespace ExpenseExplorer.Tests.Shared.Generators;

using ExpenseExplorer.API.Contract;

public record AddPurchaseRequestGenerators(
  Gen<string> ProductName,
  Gen<string> ProductCategory,
  Gen<decimal> Quantity,
  Gen<decimal> UnitPrice,
  Gen<decimal?> TotalDiscount,
  Gen<string?> Description)
{
  private static readonly Func<decimal, bool> nonNegative = d => d >= 0;
  private static readonly Func<decimal?, bool> nullOrNonNegative = d => d.HasValue == false || nonNegative(d.Value);
  private static readonly Func<string?, bool> nullOrNonEmpty = s => s == null || s.Length > 0;

  public static readonly AddPurchaseRequestGenerators valid = new(
    NonEmptyStringGenerator.NonEmptyStringGen().Generator,
    NonEmptyStringGenerator.NonEmptyStringGen().Generator,
    PositiveDecimalGenerator.PositiveDecimalGen().Generator,
    ArbMap.Default.ArbFor<decimal>().Filter(nonNegative).Generator,
    ArbMap.Default.ArbFor<decimal?>().Filter(nullOrNonNegative).Generator,
    ArbMap.Default.ArbFor<string?>().Filter(nullOrNonEmpty).Generator);

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
