using Microsoft.JSInterop;

namespace ExpenseExplorer.Web.Theming;

/// <summary>Light or dark look, remembered in the browser. Dark unless the user picked light.</summary>
public sealed class Appearance
{
    private const string StorageKey = "theme";

    public bool IsDark { get; private set; } = true;

    public event EventHandler? Changed;

    public async Task LoadAsync(IJSRuntime js) =>
        IsDark = await js.InvokeAsync<string?>("localStorage.getItem", StorageKey) != "light";

    public async Task SwitchAsync(IJSRuntime js)
    {
        IsDark = !IsDark;
        string name = IsDark ? "dark" : "light";
        await js.InvokeVoidAsync("localStorage.setItem", StorageKey, name);
        // The page background before Blazor starts follows the same choice (see index.html).
        await js.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", name);
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
