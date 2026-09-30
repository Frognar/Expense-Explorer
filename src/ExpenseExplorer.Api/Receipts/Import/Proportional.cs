namespace ExpenseExplorer.Api.Receipts.Import;

internal static class Proportional
{
    private const decimal Grosz = 0.01m;

    /// <summary>
    /// Splits <paramref name="total"/> between lines in proportion to their values, in whole grosze.
    /// Shares add up to the total exactly and no share exceeds its line's value; leftover grosze go
    /// to the lines whose exact share was cut the most (largest remainder method).
    /// Requires <c>0 &lt;= total &lt;= values.Sum()</c>.
    /// </summary>
    public static IReadOnlyList<decimal> Split(decimal total, IReadOnlyList<decimal> values)
    {
        decimal sum = values.Sum();
        if (total <= 0m || sum <= 0m)
        {
            return [.. values.Select(_ => 0m)];
        }

        decimal[] exact = [.. values.Select(value => total * value / sum)];
        decimal[] shares = [.. exact.Select(share => Math.Floor(share / Grosz) * Grosz)];
        int leftoverGrosze = (int)((total - shares.Sum()) / Grosz);

        IEnumerable<int> mostCut = Enumerable.Range(0, values.Count)
            .OrderByDescending(index => exact[index] - shares[index])
            .ThenBy(index => index)
            .Take(leftoverGrosze);
        foreach (int index in mostCut)
        {
            shares[index] += Grosz;
        }

        return shares;
    }
}
