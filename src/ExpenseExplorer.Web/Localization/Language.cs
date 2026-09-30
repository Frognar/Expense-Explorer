using System.Globalization;
using Microsoft.JSInterop;

namespace ExpenseExplorer.Web.Localization;

/// <summary>The language of the interface, remembered in the browser. Switching reloads the app.</summary>
public sealed class Language
{
    private const string StorageKey = "language";

    public static readonly IReadOnlyList<(string Code, string Name)> Available = [("pl", "Polski"), ("en", "English")];

    public string Code { get; private set; } = "pl";

    public UiText Text => Code == "en" ? UiText.English : UiText.Polish;

    public CultureInfo Culture => CultureInfo.GetCultureInfo(Code == "en" ? "en-GB" : "pl-PL");

    public async Task LoadAsync(IJSRuntime js)
    {
        string? stored = await js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        Code = Available.Any(language => language.Code == stored) ? stored! : "pl";
    }

    public static ValueTask SaveAsync(IJSRuntime js, string code) =>
        js.InvokeVoidAsync("localStorage.setItem", StorageKey, code);

    /// <summary>A message for an error code from the API; unknown codes fall back to a general message.</summary>
    public string Error(string code) =>
        Text.Errors.TryGetValue(code, out string? message) ? message : Text.UnexpectedError;

    public string Errors(IEnumerable<string> codes) => string.Join(" ", codes.Select(Error).Distinct());
}
