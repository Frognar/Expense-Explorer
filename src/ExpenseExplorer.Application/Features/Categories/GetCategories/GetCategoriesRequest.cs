using DotMaybe;
using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Categories.GetCategories;

public sealed record GetCategoriesRequest(Maybe<string> SearchTerm) : IQuery<GetCategoriesResponse>;