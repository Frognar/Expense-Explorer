namespace ExpenseExplorer.Application.Receipts;

using ExpenseExplorer.Application.Validations;

public static class PurchaseValidator
{
  public static Validated<object> Validate(string productName, string productCategory, decimal quantity)
  {
    Func<string, string, decimal, object> createResponse = (n, c, q) => new { n, c, q };
    return createResponse
      .Apply(ValidateProduct(productName))
      .Apply(ValidateCategory(productCategory))
      .Apply(ValidateQuantity(quantity));
  }

  private static Validated<string> ValidateProduct(string productName)
  {
    return string.IsNullOrWhiteSpace(productName)
      ? Validation.Failed<string>([ValidationError.Create("ProductName", "EMPTY_PRODUCT_NAME")])
      : Validation.Succeeded(productName);
  }

  private static Validated<string> ValidateCategory(string productCategory)
  {
    return string.IsNullOrWhiteSpace(productCategory)
      ? Validation.Failed<string>([ValidationError.Create("ProductCategory", "EMPTY_PRODUCT_CATEGORY")])
      : Validation.Succeeded(productCategory);
  }

  private static Validated<decimal> ValidateQuantity(decimal quantity)
  {
    return quantity <= 0
      ? Validation.Failed<decimal>([ValidationError.Create("Quantity", "NON_POSITIVE_QUANTITY")])
      : Validation.Succeeded(quantity);
  }
}
