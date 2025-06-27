using DotMaybe;
using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Stores.GetStores;

public sealed record GetStoresRequest(Maybe<string> SearchTerm) : IQuery<GetStoresResponse>;