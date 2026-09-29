using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Receipts;

/// <summary>Price of one receipt line. The discount never exceeds the gross value.</summary>
public sealed record LinePrice
{
    private LinePrice(Quantity quantity, Money unitPrice, Money discount)
    {
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
    }

    public Quantity Quantity { get; }

    public Money UnitPrice { get; }

    public Money Discount { get; }

    public Money Gross => UnitPrice.Times(Quantity);

    public Money Total => Gross.MinusClamped(Discount);

    public static Result<LinePrice> Create(Quantity quantity, Money unitPrice, Money discount)
    {
        ArgumentNullException.ThrowIfNull(quantity);
        ArgumentNullException.ThrowIfNull(unitPrice);
        ArgumentNullException.ThrowIfNull(discount);

        return discount > unitPrice.Times(quantity)
            ? Result.Failure<LinePrice>(new Error(
                "LinePrice.DiscountExceedsGross",
                "Discount cannot be greater than unit price multiplied by quantity."))
            : Result.Success(new LinePrice(quantity, unitPrice, discount));
    }
}
