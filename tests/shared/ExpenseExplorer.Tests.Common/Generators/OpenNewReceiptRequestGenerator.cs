namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public record OpenNewReceiptRequestGenerator(Gen<string> StoreName, Gen<DateOnly> PurchaseDate)
{
  public static readonly OpenNewReceiptRequestGenerator Valid = new(
    NonEmptyStringGenerator.NonEmptyStringGen().Generator,
    NonFutureDateOnlyGenerator.NonFutureDateOnlyGen().Generator);

  public Gen<OpenNewReceiptRequest> Generator
    =>
      from storeName in StoreName
      from purchaseDate in PurchaseDate
      select new OpenNewReceiptRequest(storeName, purchaseDate);

  public Arbitrary<OpenNewReceiptRequest> Arbitrary => Generator.ToArbitrary();
}
