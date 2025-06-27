using DotResult;

namespace ExpenseExplorer.Application.Features.Items.GetItems;

public interface IGetItemsPersistence
{
    Task<Result<IEnumerable<string>>> GetItemsAsync(
        IEnumerable<string> searchTerms,
        CancellationToken cancellationToken);
}