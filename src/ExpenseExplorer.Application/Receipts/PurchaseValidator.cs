namespace ExpenseExplorer.Application.Receipts;

using ExpenseExplorer.Application.Validations;

public static class PurchaseValidator
{
  public static Validated<string> Validate(string productName)
  {
    return string.IsNullOrWhiteSpace(productName)
      ? Validation.Failed<string>([ValidationError.Create("ProductName", "EMPTY_PRODUCT_NAME")])
      : Validation.Succeeded(productName);
  }
}
