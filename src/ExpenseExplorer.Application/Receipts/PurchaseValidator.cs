namespace ExpenseExplorer.Application.Receipts;

using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Application.Validations;

public static class PurchaseValidator
{
  public static Validated<object> Validate(AddPurchaseCommand command)
  {
    ArgumentNullException.ThrowIfNull(command);
    Func<string, string, decimal, decimal, decimal?, string?, object> createResponse = (pn, pc, q, up, td, d) => new
    {
      pn,
      pc,
      q,
      up,
      td,
      d,
    };

    return createResponse
      .Apply(ValidateProduct(command.ProductName))
      .Apply(ValidateCategory(command.ProductCategory))
      .Apply(ValidateQuantity(command.Quantity))
      .Apply(ValidateUnitPrice(command.UnitPrice))
      .Apply(ValidateTotalDiscount(command.TotalDiscount))
      .Apply(ValidateDescription(command.Description));
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

  private static Validated<decimal> ValidateUnitPrice(decimal unitPrice)
  {
    return unitPrice < 0
      ? Validation.Failed<decimal>([ValidationError.Create("UnitPrice", "NEGATIVE_UNIT_PRICE")])
      : Validation.Succeeded(unitPrice);
  }

  private static Validated<decimal?> ValidateTotalDiscount(decimal? totalDiscount)
  {
    return totalDiscount is < 0
      ? Validation.Failed<decimal?>([ValidationError.Create("TotalDiscount", "NEGATIVE_TOTAL_DISCOUNT")])
      : Validation.Succeeded(totalDiscount);
  }

  private static Validated<string?> ValidateDescription(string? description)
  {
    return description is not null && description.Trim().Length == 0
      ? Validation.Failed<string?>([ValidationError.Create("Description", "EMPTY_DESCRIPTION")])
      : Validation.Succeeded(description);
  }
}
