using System.Net;
using ExpenseExplorer.Contracts.ReceiptItems;
using ExpenseExplorer.Contracts.Receipts;

namespace ExpenseExplorer.Api.Tests;

public class ReceiptItemApiTests(ApiFixture api)
{
    private const string ReceiptItems = "/api/v1/receipt-items";

    private readonly HttpClient _client = api.Editor;

    [Fact]
    public async Task Lines_come_with_their_receipt_and_derived_prices()
    {
        string store = Seed.UniqueStore("Lidl");
        ReceiptResponse receipt = await _client.ReceiptAsync(
            store,
            ApiFixture.Today,
            new ReceiptItemRequest("Woda", "Media", 17.3m, 123.45m, 3.45m, "faktura"));

        ReceiptItemListResponse list = await (await api.Reader.Get($"{ReceiptItems}?stores={Uri.EscapeDataString(store)}"))
            .Read<ReceiptItemListResponse>();

        ReceiptItemSummaryResponse line = Assert.Single(list.Items.Items);
        Assert.Equal(
            new ReceiptItemSummaryResponse(
                receipt.Items[0].Id, receipt.Id, store, ApiFixture.Today, "Woda", "Media", 17.3m, 7.1358m, 123.45m, 3.45m, 120.00m, "faktura"),
            line);
        Assert.Equal(120.00m, list.TotalCost);
    }

    [Fact]
    public async Task Filters_narrow_the_lines_together()
    {
        string store = Seed.UniqueStore("Biedronka");
        await _client.ReceiptAsync(
            store,
            ApiFixture.Today.AddDays(-3),
            new ReceiptItemRequest("Mleko", "Nabiał", 2m, 6.98m, null, "Promocja 100% taniej"),
            new ReceiptItemRequest("Ser", "Nabiał", 1m, 12.99m, null, "bez promocji"),
            new ReceiptItemRequest("Chleb", "Pieczywo", 1m, 4.99m, null, "promocja"));

        ReceiptItemListResponse list = await (await _client.Get(
                $"{ReceiptItems}?stores={Uri.EscapeDataString(store)}&categories=Nabiał&totalMax=10&description=100%25"))
            .Read<ReceiptItemListResponse>();

        Assert.Equal(["Mleko"], list.Items.Items.Select(line => line.Item));
    }

    [Fact]
    public async Task Lines_can_be_sorted_and_paged_while_the_total_covers_every_page()
    {
        string store = Seed.UniqueStore("Auchan");
        await _client.ReceiptAsync(
            store,
            ApiFixture.Today,
            new ReceiptItemRequest("B", "Inne", 1m, 2.00m, null, null),
            new ReceiptItemRequest("C", "Inne", 1m, 3.00m, null, null),
            new ReceiptItemRequest("A", "Inne", 1m, 1.00m, null, null));

        ReceiptItemListResponse page = await (await _client.Get(
                $"{ReceiptItems}?stores={Uri.EscapeDataString(store)}&sortBy=Total&direction=Ascending&page=2&pageSize=2"))
            .Read<ReceiptItemListResponse>();

        Assert.Equal(["C"], page.Items.Items.Select(line => line.Item));
        Assert.Equal(3, page.Items.TotalCount);
        Assert.Equal(6.00m, page.TotalCost);
    }

    [Fact]
    public async Task Reversed_ranges_are_reported_per_field()
    {
        HttpResponseMessage response = await _client.Get($"{ReceiptItems}?totalMin=10&totalMax=1&quantityMin=5&quantityMax=1&pageSize=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            new Dictionary<string, string[]>
            {
                ["quantityMin"] = ["Input.RangeReversed"],
                ["totalMin"] = ["Input.RangeReversed"],
                ["pageSize"] = ["Input.OutOfRange"],
            },
            await response.ErrorCodes());
    }
}
