using System.Globalization;

namespace ExpenseExplorer.Web;

/// <summary>How values are shown; the current culture follows the chosen language.</summary>
public static class Display
{
    public static string Money(decimal value) => value.ToString("N2", CultureInfo.CurrentCulture) + " zł";

    /// <summary>Whole złoty, for tables and charts where the grosze would only crowd the numbers.</summary>
    public static string WholeMoney(decimal value) => value.ToString("N0", CultureInfo.CurrentCulture) + " zł";

    /// <summary>Short enough for a bar label: thousands as "k".</summary>
    public static string Compact(decimal value) =>
        value >= 1000m
            ? (value / 1000m).ToString("0.#", CultureInfo.CurrentCulture) + "k"
            : value.ToString("0", CultureInfo.CurrentCulture);

    public static string Month(DateOnly value) => value.ToString("MMM yy", CultureInfo.CurrentCulture);

    public static string Quantity(decimal value) => value.ToString("0.####", CultureInfo.CurrentCulture);

    public static string Date(DateOnly value) => value.ToString("d", CultureInfo.CurrentCulture);
}
