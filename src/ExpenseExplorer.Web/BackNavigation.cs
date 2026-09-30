using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace ExpenseExplorer.Web;

/// <summary>
/// The back arrow of a page. It returns to where the user came from, keeping that list's filters
/// and scroll position; opened straight from a link, it goes to the given page instead.
/// </summary>
public sealed class BackNavigation : IDisposable
{
    private readonly NavigationManager _navigation;
    private bool _navigatedInApp;

    public BackNavigation(NavigationManager navigation)
    {
        _navigation = navigation;
        _navigation.LocationChanged += OnLocationChanged;
    }

    public async Task GoBackAsync(IJSRuntime js, string fallback)
    {
        if (_navigatedInApp)
        {
            await js.InvokeVoidAsync("history.back");
        }
        else
        {
            _navigation.NavigateTo(fallback);
        }
    }

    public void Dispose() => _navigation.LocationChanged -= OnLocationChanged;

    private void OnLocationChanged(object? sender, LocationChangedEventArgs args) => _navigatedInApp = true;
}
