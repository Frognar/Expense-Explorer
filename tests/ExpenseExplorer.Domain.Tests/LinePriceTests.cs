using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Domain.Tests;

public class LinePriceTests
{
    [Fact]
    public void Total_is_the_amount_minus_the_discount()
    {
        var price = Given.Valid(LinePrice.Create(Given.Quantity(3m), Given.Money(7.50m), Given.Money(1.00m)));

        Assert.Equal(6.50m, price.Total.Value);
    }

    [Fact]
    public void Unit_price_is_derived_from_the_amount_and_quantity()
    {
        // A water bill: 17.3 m3 for 123.45 zl.
        var price = Given.Valid(LinePrice.Create(Given.Quantity(17.3m), Given.Money(123.45m), Money.Zero));

        Assert.Equal(7.1358m, price.UnitPrice);
        Assert.Equal(123.45m, price.Total.Value);
    }

    [Fact]
    public void Discount_cannot_exceed_the_amount()
    {
        var result = LinePrice.Create(Given.Quantity(2m), Given.Money(2.00m), Given.Money(2.01m));

        Assert.Equal("LinePrice.DiscountExceedsAmount", Assert.Single(result.Errors).Code);
    }

    [Fact]
    public void Discount_can_equal_the_amount()
    {
        var price = Given.Valid(LinePrice.Create(Given.Quantity(2m), Given.Money(2.00m), Given.Money(2.00m)));

        Assert.Equal(Money.Zero, price.Total);
    }
}
