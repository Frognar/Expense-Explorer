using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Domain.Tests;

public class ValueObjectTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void StoreName_rejects_blank_values(string? value)
    {
        var result = StoreName.Create(value);

        Assert.Equal("StoreName.Empty", Assert.Single(result.Errors).Code);
    }

    [Fact]
    public void StoreName_is_trimmed()
    {
        var store = Given.Valid(StoreName.Create("  Lidl "));

        Assert.Equal("Lidl", store.Value);
    }

    [Fact]
    public void StoreName_rejects_values_longer_than_the_limit()
    {
        var result = StoreName.Create(new string('a', StoreName.MaxLength + 1));

        Assert.Equal("StoreName.TooLong", Assert.Single(result.Errors).Code);
    }

    [Fact]
    public void Description_treats_blank_input_as_missing()
    {
        var description = Given.Valid(Description.CreateOptional("  "));

        Assert.Null(description);
    }

    [Theory]
    [InlineData("-0.01", "Money.Negative")]
    [InlineData("1.001", "Money.TooPrecise")]
    public void Money_rejects_invalid_amounts(string value, string code)
    {
        var result = Money.Create(decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture));

        Assert.Equal(code, Assert.Single(result.Errors).Code);
    }

    [Fact]
    public void Money_accepts_trailing_zeros_beyond_two_decimal_places()
    {
        Assert.True(Money.Create(1.500m).IsSuccess);
    }

    [Theory]
    [InlineData("-0.0001", "UnitPrice.Negative")]
    [InlineData("7.13589", "UnitPrice.TooPrecise")]
    public void UnitPrice_rejects_invalid_values(string value, string code)
    {
        var result = UnitPrice.Create(decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture));

        Assert.Equal(code, Assert.Single(result.Errors).Code);
    }

    [Fact]
    public void UnitPrice_accepts_four_decimal_places()
    {
        Assert.True(UnitPrice.Create(7.1359m).IsSuccess);
    }

    [Theory]
    [InlineData("0", "Quantity.NotPositive")]
    [InlineData("-1", "Quantity.NotPositive")]
    [InlineData("0.00001", "Quantity.TooPrecise")]
    public void Quantity_rejects_invalid_values(string value, string code)
    {
        var result = Quantity.Create(decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture));

        Assert.Equal(code, Assert.Single(result.Errors).Code);
    }

    [Fact]
    public void PurchaseDate_cannot_be_in_the_future()
    {
        var result = PurchaseDate.Create(Given.Today.AddDays(1), Given.Today);

        Assert.Equal("PurchaseDate.InFuture", Assert.Single(result.Errors).Code);
    }

    [Fact]
    public void PurchaseDate_can_be_today()
    {
        Assert.True(PurchaseDate.Create(Given.Today, Given.Today).IsSuccess);
    }

    [Fact]
    public void Identifiers_cannot_be_empty()
    {
        Assert.False(ReceiptId.Create(Guid.Empty).IsSuccess);
        Assert.False(ReceiptItemId.Create(Guid.Empty).IsSuccess);
    }
}
