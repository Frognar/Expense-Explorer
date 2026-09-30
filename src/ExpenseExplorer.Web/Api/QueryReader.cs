using System.Collections.Specialized;
using System.Globalization;
using System.Web;

namespace ExpenseExplorer.Web.Api;

/// <summary>Reads values written by <see cref="QueryString"/>; anything unreadable counts as missing.</summary>
public sealed class QueryReader(string query)
{
    private readonly NameValueCollection _values = HttpUtility.ParseQueryString(query);

    public static QueryReader Of(Uri uri) => new(uri.Query);

    public string? Text(string name) => _values[name] is { Length: > 0 } value ? value : null;

    public string[]? Texts(string name) => _values.GetValues(name) is { Length: > 0 } values ? values : null;

    public DateOnly? Date(string name) =>
        DateOnly.TryParseExact(Text(name), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly value)
            ? value
            : null;

    public decimal? Number(string name) =>
        decimal.TryParse(Text(name), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value) ? value : null;

    public int? WholeNumber(string name) =>
        int.TryParse(Text(name), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) ? value : null;

    public TEnum? Enum<TEnum>(string name)
        where TEnum : struct, Enum =>
        System.Enum.TryParse(Text(name), ignoreCase: true, out TEnum value) && System.Enum.IsDefined(value) ? value : null;
}
