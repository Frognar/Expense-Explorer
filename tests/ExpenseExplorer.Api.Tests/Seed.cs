using ExpenseExplorer.Contracts.Receipts;

namespace ExpenseExplorer.Api.Tests;

internal static class Seed
{
    /// <summary>A store name no other test uses, so filters by store see only this test's data.</summary>
    public static string UniqueStore(string prefix) => $"{prefix} {Guid.NewGuid():N}";

    public static async Task<ReceiptResponse> ReceiptAsync(
        this HttpClient client,
        string store,
        DateOnly purchaseDate,
        params ReceiptItemRequest[] items)
    {
        ReceiptResponse receipt = await (await client.Post("/api/v1/receipts", new CreateReceiptRequest(store, purchaseDate)))
            .Read<ReceiptResponse>();
        foreach (ReceiptItemRequest item in items)
        {
            (await client.Post($"/api/v1/receipts/{receipt.Id}/items", item)).EnsureSuccessStatusCode();
        }

        return await (await client.Get($"/api/v1/receipts/{receipt.Id}")).Read<ReceiptResponse>();
    }
}
