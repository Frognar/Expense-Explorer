using Microsoft.AspNetCore.Components;

namespace ExpenseExplorer.Web;

public static class NavigationExtensions
{
    /// <summary>Whether the current address is the given page, whatever its query string.</summary>
    public static bool IsOn(this NavigationManager navigation, string page) =>
        navigation.ToBaseRelativePath(new Uri(navigation.Uri).GetLeftPart(UriPartial.Path)) == page.Split('?')[0];
}
