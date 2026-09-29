using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Domain.Tests;

public class LinePriceTests
{
    [Fact]
    public void Total_is_unit_price_times_quantity_minus_discount()
    {
        var price = Given.Valid(LinePrice.Create(Given.Quantity(3m), Given.Money(2.50m), Given.Money(1.00m)));

        Assert.Equal(7.50m, price.Gross.Value);
        Assert.Equal(6.50m, price.Total.Value);
    }

    [Fact]
    public void Gross_is_rounded_half_away_from_zero_to_two_decimal_places()
    {
        var price = Given.Valid(LinePrice.Create(Given.Quantity(0.335m), Given.Money(10.00m), Money.Zero));

        Assert.Equal(3.35m, price.Gross.Value);
    }

    [Fact]
    public void Discount_cannot_exceed_gross_value()
    {
        var result = LinePrice.Create(Given.Quantity(2m), Given.Money(1.00m), Given.Money(2.01m));

        Assert.Equal("LinePrice.DiscountExceedsGross", Assert.Single(result.Errors).Code);
    }

    [Fact]
    public void Discount_can_equal_gross_value()
    {
        var price = Given.Valid(LinePrice.Create(Given.Quantity(2m), Given.Money(1.00m), Given.Money(2.00m)));

        Assert.Equal(Money.Zero, price.Total);
    }
}
