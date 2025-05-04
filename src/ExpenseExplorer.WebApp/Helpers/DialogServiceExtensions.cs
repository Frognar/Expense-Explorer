using Radzen;

namespace ExpenseExplorer.WebApp.Helpers;

internal static class DialogServiceExtensions
{
    public static async Task<Unit> OpenErrorDialogAsync(this DialogService dialogService, string title, string message)
    {
        await dialogService.OpenAsync("", _ =>
        {
            return b =>
            {
                b.OpenElement(0, "RadzenRow");
                b.OpenElement(1, "RadzenColumn");
                b.AddAttribute(2, "Size", "12");
                b.AddContent(3, message);
                b.CloseElement();
                b.CloseElement();
            };

        }, new DialogOptions
        {
            ShowTitle = false,
            Style = "min-height:auto;min-width:auto;width:auto",
            CloseDialogOnEsc = false
        });

        return Unit.Instance;
    }
}