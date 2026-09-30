using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Receipts;

/// <summary>
/// Price of one receipt line as printed on the receipt or invoice: the amount for the whole
/// quantity and a discount on that amount. The discount never exceeds the amount.
/// </summary>
public sealed record LinePrice
{
    public const int UnitPriceDecimalPlaces = 4;

    private LinePrice(Quantity quantity, Money amount, Money discount)
    {
        Quantity = quantity;
        Amount = amount;
        Discount = discount;
    }

    public Quantity Quantity { get; }

    /// <summary>Amount for the whole quantity, before the discount.</summary>
    public Money Amount { get; }

    public Money Discount { get; }

    /// <summary>What was actually paid for the line.</summary>
    public Money Total => Amount.MinusClamped(Discount);

    /// <summary>Derived for display only; rounded, so it may not multiply back to <see cref="Amount"/> exactly.</summary>
    public decimal UnitPrice =>
        decimal.Round(Amount.Value / Quantity.Value, UnitPriceDecimalPlaces, MidpointRounding.AwayFromZero);

    public static Result<LinePrice> Create(Quantity quantity, Money amount, Money discount)
    {
        ArgumentNullException.ThrowIfNull(quantity);
        ArgumentNullException.ThrowIfNull(amount);
        ArgumentNullException.ThrowIfNull(discount);

        return discount > amount
            ? Result.Failure<LinePrice>(new Error(
                "LinePrice.DiscountExceedsAmount",
                "Discount cannot be greater than the amount for the whole quantity."))
            : Result.Success(new LinePrice(quantity, amount, discount));
    }
}
