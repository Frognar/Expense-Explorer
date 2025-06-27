using DotResult;

namespace ExpenseExplorer.Application.Features.Categories.GetCategories;

public interface IGetCategoriesPersistence
{
    Task<Result<IEnumerable<string>>> GetCategoriesAsync(
        IEnumerable<string> searchTerms,
        CancellationToken cancellationToken);
}