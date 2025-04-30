namespace ExpenseExplorer.WebApp.Models;

internal sealed record ReceiptDetails(
    Guid Id,
    string Store,
    DateOnly PurchaseDate,
    decimal TotalCost);