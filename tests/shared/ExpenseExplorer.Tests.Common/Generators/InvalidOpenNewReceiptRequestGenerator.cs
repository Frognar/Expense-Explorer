namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public static class InvalidOpenNewReceiptRequestGenerator
{
  public static Arbitrary<OpenNewReceiptRequest> InvalidOpenNewReceiptRequestGen()
  {
    var invalidStoreName = EmptyStringGenerator.EmptyStringGen().Generator;
    var invalidPurchaseDate = FutureDateOnlyGenerator.FutureDateOnlyGen().Generator;

    return Gen.OneOf(
        (OpenNewReceiptRequestGenerator.Valid with { StoreName = invalidStoreName }).Generator,
        (OpenNewReceiptRequestGenerator.Valid with { PurchaseDate = invalidPurchaseDate }).Generator,
        new OpenNewReceiptRequestGenerator(invalidStoreName, invalidPurchaseDate).Generator)
      .ToArbitrary();
  }
}
