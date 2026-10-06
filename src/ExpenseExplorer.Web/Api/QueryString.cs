using System.Globalization;
using System.Text;

namespace ExpenseExplorer.Web.Api;

/// <summary>Builds a query string; empty values are left out, lists repeat their name.</summary>
public sealed class QueryString
{
    private readonly List<string> _parts = [];

    public QueryString Add(string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            _parts.Add($"{name}={Uri.EscapeDataString(value)}");
        }

        return this;
    }

    public QueryString Add(string name, IEnumerable<string>? values)
    {
        foreach (string value in values ?? [])
        {
            Add(name, value);
        }

        return this;
    }

    public QueryString Add(string name, decimal? value) =>
        Add(name, value?.ToString(CultureInfo.InvariantCulture));

    public QueryString Add(string name, int? value) =>
        Add(name, value?.ToString(CultureInfo.InvariantCulture));

    public QueryString Add(string name, DateOnly? value) =>
        Add(name, value?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

    public QueryString Add<TEnum>(string name, TEnum? value)
        where TEnum : struct, Enum =>
        Add(name, value?.ToString());

    /// <summary>Writes the name even without a date, so "no date" can be told apart from "not given".</summary>
    public QueryString AddKeepingName(string name, DateOnly? value)
    {
        if (value is null)
        {
            _parts.Add($"{name}=");
            return this;
        }

        return Add(name, value);
    }

    public override string ToString() =>
        _parts.Count == 0 ? "" : new StringBuilder("?").AppendJoin('&', _parts).ToString();
}
