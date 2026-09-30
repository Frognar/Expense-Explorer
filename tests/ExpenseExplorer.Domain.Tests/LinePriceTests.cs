using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Domain.Tests;

public class LinePriceTests
{
    [Fact]
    public void Total_is_unit_price_times_quantity_minus_discount()
    {
        var price = Given.Valid(LinePrice.Create(Given.Quantity(3m), Given.UnitPrice(2.50m), Given.Money(1.00m)));

        Assert.Equal(7.50m, price.Gross.Value);
        Assert.Equal(6.50m, price.Total.Value);
    }

    [Fact]
    public void Gross_is_rounded_half_away_from_zero_to_two_decimal_places()
    {
        var price = Given.Valid(LinePrice.Create(Given.Quantity(0.335m), Given.UnitPrice(10.00m), Money.Zero));

        Assert.Equal(3.35m, price.Gross.Value);
    }

    [Fact]
    public void Discount_cannot_exceed_gross_value()
    {
        var result = LinePrice.Create(Given.Quantity(2m), Given.UnitPrice(1.00m), Given.Money(2.01m));

        Assert.Equal("LinePrice.DiscountExceedsGross", Assert.Single(result.Errors).Code);
    }

    [Fact]
    public void Discount_can_equal_gross_value()
    {
        var price = Given.Valid(LinePrice.Create(Given.Quantity(2m), Given.UnitPrice(1.00m), Given.Money(2.00m)));

        Assert.Equal(Money.Zero, price.Total);
    }

    [Fact]
    public void Unit_price_with_four_decimal_places_gives_gross_in_whole_grosze()
    {
        // A water bill: 17.3 m3 for 123.45 zl, unit price entered as 123.45 / 17.3 rounded to 4 places.
        var price = Given.Valid(LinePrice.Create(Given.Quantity(17.3m), Given.UnitPrice(7.1358m), Money.Zero));

        Assert.Equal(123.45m, price.Gross.Value);
    }
}
