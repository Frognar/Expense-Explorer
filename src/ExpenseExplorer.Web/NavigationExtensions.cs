using Microsoft.AspNetCore.Components;

namespace ExpenseExplorer.Web;

public static class NavigationExtensions
{
    /// <summary>Whether the current address is the given page, whatever its query string.</summary>
    public static bool IsOn(this NavigationManager navigation, string page) =>
        navigation.ToBaseRelativePath(new Uri(navigation.Uri).GetLeftPart(UriPartial.Path)) == PathOf(page);

    private static string PathOf(string page)
    {
        string path = page.Split('?')[0];
        return path.StartsWith("./", StringComparison.Ordinal) ? path[2..] : path;
    }
}
