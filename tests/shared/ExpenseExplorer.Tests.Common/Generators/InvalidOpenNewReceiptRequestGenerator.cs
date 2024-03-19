namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public static class InvalidOpenNewReceiptRequestGenerator
{
  public static Arbitrary<OpenNewReceiptRequest> InvalidOpenNewReceiptRequestGen()
  {
    var invalidStoreName =
      from storeName in EmptyStringGenerator.EmptyStringGen().Generator
      from purchaseDate in NonFutureDateOnlyGenerator.NonFutureDateOnlyGen().Generator
      select new OpenNewReceiptRequest(storeName, purchaseDate);

    var invalidPurchaseDate =
      from storeName in NonEmptyStringGenerator.NonEmptyStringGen().Generator
      from purchaseDate in FutureDateOnlyGenerator.FutureDateOnlyGen().Generator
      select new OpenNewReceiptRequest(storeName, purchaseDate);

    var invalidStoreNameAndPurchaseDate =
      from storeName in EmptyStringGenerator.EmptyStringGen().Generator
      from purchaseDate in FutureDateOnlyGenerator.FutureDateOnlyGen().Generator
      select new OpenNewReceiptRequest(storeName, purchaseDate);

    return Gen.OneOf(
        invalidStoreName,
        invalidPurchaseDate,
        invalidStoreNameAndPurchaseDate)
      .ToArbitrary();
  }
}
