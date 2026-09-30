using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Domain.Tests;

internal static class Given
{
    public static readonly DateOnly Today = new(2026, 9, 29);

    public static T Valid<T>(Result<T> result) =>
        result.Match(
            value => value,
            errors => throw new InvalidOperationException($"Expected success, got {string.Join(", ", errors)}"));

    public static Money Money(decimal value) => Valid(Receipts.Money.Create(value));

    public static UnitPrice UnitPrice(decimal value) => Valid(Receipts.UnitPrice.Create(value));

    public static Quantity Quantity(decimal value) => Valid(Receipts.Quantity.Create(value));

    public static PurchaseDate Date(DateOnly value) => Valid(PurchaseDate.Create(value, Today));

    public static Purchase Purchase(
        string item = "Milk",
        string category = "Food",
        decimal quantity = 1m,
        decimal unitPrice = 3.49m,
        decimal discount = 0m) =>
        new(
            Valid(ItemName.Create(item)),
            Valid(CategoryName.Create(category)),
            Valid(LinePrice.Create(Quantity(quantity), UnitPrice(unitPrice), Money(discount))),
            description: null);

    public static Receipt Receipt() =>
        Receipts.Receipt.Create(ReceiptId.New(), Valid(StoreName.Create("Biedronka")), Date(Today));
}
