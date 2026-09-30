using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Domain.Tests;

public class ResultTests
{
    [Fact]
    public void Combine_collects_errors_from_every_failed_input()
    {
        var result = ResultCombine.Combine(
            StoreName.Create("").ForTarget("store"),
            Money.Create(-1m).ForTarget("price"),
            Quantity.Create(1m),
            (store, price, quantity) => (store, price, quantity));

        Assert.Equal(
            [("StoreName.Empty", "store"), ("Money.Negative", "price")],
            result.Errors.Select(error => (error.Code, error.Target)));
    }

    [Fact]
    public void Combine_passes_values_when_all_inputs_succeed()
    {
        var result = ResultCombine.Combine(
            StoreName.Create("Lidl"),
            Money.Create(1m),
            (store, price) => $"{store} {price}");

        Assert.Equal("Lidl 1.00", Given.Valid(result));
    }

    [Fact]
    public void Failure_without_errors_is_not_allowed()
    {
        Assert.Throws<ArgumentException>(() => Result.Failure<int>([]));
    }
}
