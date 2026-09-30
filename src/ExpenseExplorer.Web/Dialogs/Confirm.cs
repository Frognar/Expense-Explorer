using ExpenseExplorer.Web.Localization;
using MudBlazor;

namespace ExpenseExplorer.Web.Dialogs;

public static class Confirm
{
    /// <summary>Asks before something that cannot be undone; true only when the user agrees.</summary>
    public static async Task<bool> AskAsync(IDialogService dialogs, UiText text, string question) =>
        await dialogs.ShowMessageBoxAsync(
            title: null,
            message: question,
            yesText: text.Delete,
            cancelText: text.Cancel) == true;
}
