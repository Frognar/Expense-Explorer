namespace ExpenseExplorer.Application.Features.Categories.GetCategories;

public sealed record GetCategoriesResponse(IEnumerable<string> Categories);