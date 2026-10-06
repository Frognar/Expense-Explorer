using System.Net;
using ExpenseExplorer.Contracts.Common;
using ExpenseExplorer.Contracts.Receipts;

namespace ExpenseExplorer.Api.Tests;

public class ReceiptApiTests(ApiFixture api)
{
    private const string Receipts = "/api/v1/receipts";

    private static readonly DateOnly Today = ApiFixture.Today;

    private readonly HttpClient _client = api.Editor;

    [Fact]
    public async Task Create_returns_the_new_receipt_and_its_location()
    {
        HttpResponseMessage response = await _client.Post(Receipts, new CreateReceiptRequest("  Lidl ", Today));
        ReceiptResponse receipt = await response.Read<ReceiptResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"{Receipts}/{receipt.Id}", response.Headers.Location?.ToString());
        Assert.Equal("Lidl", receipt.Store);
        Assert.Equal(0m, receipt.Total);
    }

    [Fact]
    public async Task Create_reports_every_invalid_field_at_once()
    {
        HttpResponseMessage response = await _client.Post(Receipts, new CreateReceiptRequest(" ", null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            new Dictionary<string, string[]>
            {
                ["store"] = ["StoreName.Empty"],
                ["purchaseDate"] = ["Input.Required"],
            },
            await response.ErrorCodes());
    }

    [Fact]
    public async Task Create_rejects_a_purchase_date_in_the_future()
    {
        HttpResponseMessage response = await _client.Post(Receipts, new CreateReceiptRequest("Lidl", Today.AddDays(1)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(["PurchaseDate.InFuture"], (await response.ErrorCodes())["purchaseDate"]);
    }

    [Fact]
    public async Task Get_unknown_receipt_returns_not_found_problem()
    {
        HttpResponseMessage response = await _client.Get($"{Receipts}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(["Receipt.NotFound"], (await response.ErrorCodes())[""]);
    }

    [Fact]
    public async Task Items_added_to_a_receipt_are_returned_with_totals()
    {
        ReceiptResponse receipt = await CreateReceiptAsync("Biedronka");

        await AddItemAsync(receipt.Id, new ReceiptItemRequest("Mleko", "Nabiał", 2m, 6.98m, 0.50m, null));
        await AddItemAsync(receipt.Id, new ReceiptItemRequest("Chleb", "Pieczywo", 0.5m, 5.00m, null, "  pół bochenka "));
        ReceiptResponse stored = await (await _client.Get($"{Receipts}/{receipt.Id}")).Read<ReceiptResponse>();

        Assert.Equal(11.48m, stored.Total);
        Assert.Equal(0.50m, stored.Discount);
        Assert.Collection(
            stored.Items,
            milk =>
            {
                Assert.Equal("Mleko", milk.Item);
                Assert.Equal(6.98m, milk.Amount);
                Assert.Equal(3.49m, milk.UnitPrice);
                Assert.Equal(6.48m, milk.Total);
            },
            bread =>
            {
                Assert.Equal(5.00m, bread.Total);
                Assert.Equal(10.00m, bread.UnitPrice);
                Assert.Equal(0m, bread.Discount);
                Assert.Equal("pół bochenka", bread.Description);
            });
    }

    [Fact]
    public async Task Adding_an_item_reports_every_invalid_field_at_once()
    {
        ReceiptResponse receipt = await CreateReceiptAsync("Lidl");

        HttpResponseMessage response = await _client.Post(
            $"{Receipts}/{receipt.Id}/items",
            new ReceiptItemRequest("", null, 0m, null, -1m, null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            new Dictionary<string, string[]>
            {
                ["item"] = ["ItemName.Empty"],
                ["category"] = ["CategoryName.Empty"],
                ["quantity"] = ["Quantity.NotPositive"],
                ["amount"] = ["Input.Required"],
                ["discount"] = ["Money.Negative"],
            },
            await response.ErrorCodes());
    }

    [Fact]
    public async Task Utility_bill_line_keeps_the_invoice_amount_and_derives_the_unit_price()
    {
        ReceiptResponse receipt = await CreateReceiptAsync("Wodociągi");

        HttpResponseMessage response = await _client.Post(
            $"{Receipts}/{receipt.Id}/items",
            new ReceiptItemRequest("Woda", "Media", 17.3m, 123.45m, null, null));
        ReceiptItemResponse water = await response.Read<ReceiptItemResponse>();

        Assert.Equal(123.45m, water.Total);
        Assert.Equal(7.1358m, water.UnitPrice);
    }

    [Fact]
    public async Task Amount_with_more_than_two_decimal_places_is_rejected()
    {
        ReceiptResponse receipt = await CreateReceiptAsync("Lidl");

        HttpResponseMessage response = await _client.Post(
            $"{Receipts}/{receipt.Id}/items",
            new ReceiptItemRequest("Ser", "Nabiał", 1m, 5.001m, null, null));

        Assert.Equal(["Money.TooPrecise"], (await response.ErrorCodes())["amount"]);
    }

    [Fact]
    public async Task Discount_above_the_amount_is_rejected()
    {
        ReceiptResponse receipt = await CreateReceiptAsync("Lidl");

        HttpResponseMessage response = await _client.Post(
            $"{Receipts}/{receipt.Id}/items",
            new ReceiptItemRequest("Masło", "Nabiał", 1m, 5.00m, 6m, null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(["LinePrice.DiscountExceedsAmount"], (await response.ErrorCodes())["discount"]);
    }

    [Fact]
    public async Task Adding_an_item_to_an_unknown_receipt_returns_not_found()
    {
        HttpResponseMessage response = await _client.Post(
            $"{Receipts}/{Guid.NewGuid()}/items",
            new ReceiptItemRequest("Masło", "Nabiał", 1m, 5.00m, null, null));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Items_can_be_changed_and_removed()
    {
        ReceiptResponse receipt = await CreateReceiptAsync("Lidl");
        ReceiptItemResponse first = await AddItemAsync(receipt.Id, new ReceiptItemRequest("Masło", "Nabiał", 1m, 5.00m, null, null));
        ReceiptItemResponse second = await AddItemAsync(receipt.Id, new ReceiptItemRequest("Ser", "Nabiał", 1m, 8.00m, null, null));

        HttpResponseMessage changed = await _client.Put(
            $"{Receipts}/{receipt.Id}/items/{first.Id}",
            new ReceiptItemRequest("Masło extra", "Nabiał", 2m, 12.00m, 1m, null));
        HttpResponseMessage removed = await _client.Delete($"{Receipts}/{receipt.Id}/items/{second.Id}");
        ReceiptResponse stored = await (await _client.Get($"{Receipts}/{receipt.Id}")).Read<ReceiptResponse>();

        Assert.Equal(HttpStatusCode.OK, changed.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, removed.StatusCode);
        ReceiptItemResponse remaining = Assert.Single(stored.Items);
        Assert.Equal(first.Id, remaining.Id);
        Assert.Equal("Masło extra", remaining.Item);
        Assert.Equal(11m, stored.Total);
    }

    [Fact]
    public async Task Removing_an_unknown_item_returns_not_found()
    {
        ReceiptResponse receipt = await CreateReceiptAsync("Lidl");

        HttpResponseMessage response = await _client.Delete($"{Receipts}/{receipt.Id}/items/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(["Receipt.ItemNotFound"], (await response.ErrorCodes())[""]);
    }

    [Fact]
    public async Task Patch_changes_only_the_fields_that_were_sent()
    {
        ReceiptResponse receipt = await CreateReceiptAsync("Lidl");

        HttpResponseMessage response = await _client.Patch($"{Receipts}/{receipt.Id}", new UpdateReceiptRequest("Aldi", null));
        ReceiptResponse changed = await response.Read<ReceiptResponse>();

        Assert.Equal("Aldi", changed.Store);
        Assert.Equal(receipt.PurchaseDate, changed.PurchaseDate);
    }

    [Fact]
    public async Task Patch_rejects_a_blank_store()
    {
        ReceiptResponse receipt = await CreateReceiptAsync("Lidl");

        HttpResponseMessage response = await _client.Patch($"{Receipts}/{receipt.Id}", new UpdateReceiptRequest("", null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Deleted_receipt_is_gone_with_its_items()
    {
        ReceiptResponse receipt = await CreateReceiptAsync("Lidl");
        await AddItemAsync(receipt.Id, new ReceiptItemRequest("Masło", "Nabiał", 1m, 5.00m, null, null));

        HttpResponseMessage deleted = await _client.Delete($"{Receipts}/{receipt.Id}");
        HttpResponseMessage fetched = await _client.Get($"{Receipts}/{receipt.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, fetched.StatusCode);
    }

    [Fact]
    public async Task Duplicate_copies_store_and_items_to_a_new_date()
    {
        ReceiptResponse receipt = await CreateReceiptAsync("Lidl");
        await AddItemAsync(receipt.Id, new ReceiptItemRequest("Masło", "Nabiał", 1m, 5.00m, null, null));

        HttpResponseMessage response = await _client.Post(
            $"{Receipts}/{receipt.Id}/duplicate",
            new DuplicateReceiptRequest(Today.AddDays(-3)));
        ReceiptResponse copy = await response.Read<ReceiptResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotEqual(receipt.Id, copy.Id);
        Assert.Equal(Today.AddDays(-3), copy.PurchaseDate);
        Assert.Equal("Masło", Assert.Single(copy.Items).Item);
    }

    [Fact]
    public async Task List_filters_sorts_and_sums_the_matching_receipts()
    {
        string store = $"Sklep-{Guid.NewGuid():N}";
        ReceiptResponse cheap = await CreateReceiptAsync(store, Today.AddDays(-2));
        await AddItemAsync(cheap.Id, new ReceiptItemRequest("Woda", "Napoje", 1m, 2.50m, 0.50m, null));
        ReceiptResponse expensive = await CreateReceiptAsync(store, Today.AddDays(-1));
        await AddItemAsync(expensive.Id, new ReceiptItemRequest("Kawa", "Napoje", 1m, 30.00m, null, null));
        ReceiptResponse old = await CreateReceiptAsync(store, Today.AddDays(-30));
        await AddItemAsync(old.Id, new ReceiptItemRequest("Herbata", "Napoje", 1m, 10.00m, null, null));

        HttpResponseMessage response = await _client.Get(
            $"{Receipts}?stores={store}&from={Today.AddDays(-7):yyyy-MM-dd}&sortBy=Total&direction=Descending&pageSize=1");
        ReceiptListResponse list = await response.Read<ReceiptListResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, list.Receipts.TotalCount);
        Assert.Equal(32m, list.TotalCost);
        Assert.Equal(0.50m, list.TotalDiscount);
        Assert.Equal(expensive.Id, Assert.Single(list.Receipts.Items).Id);
    }

    [Fact]
    public async Task List_totals_round_the_same_way_as_receipt_details()
    {
        string store = $"Sklep-{Guid.NewGuid():N}";
        ReceiptResponse receipt = await CreateReceiptAsync(store);
        await AddItemAsync(receipt.Id, new ReceiptItemRequest("Ser", "Nabiał", 0.5m, 5.00m, null, null));

        ReceiptListResponse list = await (await _client.Get($"{Receipts}?stores={store}")).Read<ReceiptListResponse>();
        ReceiptResponse details = await (await _client.Get($"{Receipts}/{receipt.Id}")).Read<ReceiptResponse>();

        Assert.Equal(5.00m, details.Total);
        Assert.Equal(details.Total, Assert.Single(list.Receipts.Items).Total);
    }

    [Fact]
    public async Task List_rejects_invalid_paging_and_reversed_ranges()
    {
        HttpResponseMessage response = await _client.Get($"{Receipts}?page=0&pageSize=500&totalMin=10&totalMax=1");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(["page", "pageSize", "totalMin"], (await response.ErrorCodes()).Keys.Order());
    }

    [Fact]
    public async Task Dictionaries_return_distinct_names_matching_the_search()
    {
        string marker = Guid.NewGuid().ToString("N")[..8];
        ReceiptResponse receipt = await CreateReceiptAsync($"Żabka {marker}");
        await AddItemAsync(receipt.Id, new ReceiptItemRequest($"Bułka {marker}", $"Pieczywo {marker}", 1m, 1.00m, null, null));
        await AddItemAsync(receipt.Id, new ReceiptItemRequest($"Bułka {marker}", $"Pieczywo {marker}", 2m, 2.00m, null, null));

        string[] stores = await (await _client.Get($"/api/v1/stores?search={marker.ToUpperInvariant()}")).Read<string[]>();
        string[] items = await (await _client.Get($"/api/v1/items?search={marker}")).Read<string[]>();
        string[] categories = await (await _client.Get($"/api/v1/categories?search={marker}")).Read<string[]>();

        Assert.Equal([$"Żabka {marker}"], stores);
        Assert.Equal([$"Bułka {marker}"], items);
        Assert.Equal([$"Pieczywo {marker}"], categories);
    }

    private async Task<ReceiptResponse> CreateReceiptAsync(string store, DateOnly? purchaseDate = null)
    {
        HttpResponseMessage response = await _client.Post(Receipts, new CreateReceiptRequest(store, purchaseDate ?? Today));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await response.Read<ReceiptResponse>();
    }

    private async Task<ReceiptItemResponse> AddItemAsync(Guid receiptId, ReceiptItemRequest item)
    {
        HttpResponseMessage response = await _client.Post($"{Receipts}/{receiptId}/items", item);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await response.Read<ReceiptItemResponse>();
    }
}
