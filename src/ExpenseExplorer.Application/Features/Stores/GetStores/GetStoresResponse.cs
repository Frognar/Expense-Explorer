namespace ExpenseExplorer.Application.Features.Stores.GetStores;

public sealed record GetStoresResponse(IEnumerable<string> Stores);