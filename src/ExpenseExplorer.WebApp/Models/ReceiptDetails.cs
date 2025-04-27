namespace ExpenseExplorer.WebApp.Models;

internal sealed record ReceiptDetails(
    int Id,
    string Store,
    DateOnly PurchaseDate,
    decimal TotalCost);