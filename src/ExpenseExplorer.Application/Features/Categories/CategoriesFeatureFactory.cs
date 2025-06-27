using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Features.Categories.GetCategories;

namespace ExpenseExplorer.Application.Features.Categories;

public static class CategoriesFeatureFactory
{
    public static IQueryHandler<GetCategoriesRequest, GetCategoriesResponse>
        CreateGetCategoriesCommandHandler(IGetCategoriesPersistence getCategoriesPersistence)
        => new GetCategoriesCommandHandler(getCategoriesPersistence);
}