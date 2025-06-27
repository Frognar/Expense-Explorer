using DotMaybe;
using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Items.GetItems;

public sealed record GetItemsRequest(Maybe<string> SearchTerm) : IQuery<GetItemsResponse>;