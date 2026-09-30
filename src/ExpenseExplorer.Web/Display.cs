using System.Globalization;

namespace ExpenseExplorer.Web;

/// <summary>How values are shown; the current culture follows the chosen language.</summary>
public static class Display
{
    public static string Money(decimal value) => value.ToString("N2", CultureInfo.CurrentCulture) + " zł";

    public static string Quantity(decimal value) => value.ToString("0.####", CultureInfo.CurrentCulture);

    public static string Date(DateOnly value) => value.ToString("d", CultureInfo.CurrentCulture);
}
