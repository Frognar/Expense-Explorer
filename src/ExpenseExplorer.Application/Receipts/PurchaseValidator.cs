namespace ExpenseExplorer.Application.Receipts;

using ExpenseExplorer.Application.Validations;

public static class PurchaseValidator
{
  public static Validated<object> Validate(string productName, string productCategory)
  {
    Func<string, string, object> createResponse = (name, category) => new { name, category };
    return createResponse
      .Apply(ValidateProduct(productName))
      .Apply(ValidateCategory(productCategory));
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
}
