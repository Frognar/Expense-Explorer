namespace ExpenseExplorer.Application.Features.Items.GetItems;

public sealed record GetItemsResponse(IEnumerable<string> Items);