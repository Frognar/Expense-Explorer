using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Application.Dictionaries;

public static class DictionaryUseCases
{
    public static readonly Error NameNotFound = new("Dictionary.NameNotFound", "No receipt uses this name.", ErrorType.NotFound);

    public static async Task<Result<RenamedName>> RenameAsync(
        IDictionaryEditor editor,
        RenameName command,
        CancellationToken cancellationToken) =>
        await editor.RenameAsync(command, cancellationToken) is { } renamed
            ? Result.Success(renamed)
            : Result.Failure<RenamedName>(NameNotFound.For("from"));
}
