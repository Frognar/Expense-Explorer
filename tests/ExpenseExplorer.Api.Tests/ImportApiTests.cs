using System.Net;
using System.Net.Http.Headers;
using System.Text;
using ExpenseExplorer.Api.Receipts.Import;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Contracts.Receipts;
using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Api.Tests;

public class ImportApiTests(ApiFixture api)
{
    private const string Import = "/api/v1/receipts/import/biedronka";

    /// <summary>
    /// Milk with a 1.00 discount, bread with the VAT letter after its name, cheese by weight with
    /// a 10% discount, and a 2.00 voucher for the whole receipt.
    /// </summary>
    private const string Receipt = """
        {
          "header": [ { "headerData": { "date": "2026-09-28T22:30:00.000Z" } } ],
          "body": [
            { "sellLine": { "name": "Mleko  2%", "vatId": "C", "quantity": "2", "price": 349 } },
            { "discountLine": { "isDiscount": true, "isPercent": false, "value": 100 } },
            { "sellLine": { "name": "Chleb razowy C", "vatId": "C", "quantity": "1", "price": "499" } },
            { "sellLine": { "name": "Ser gouda", "vatId": "C", "quantity": "0,512", "price": 2999 } },
            { "discountLine": { "isDiscount": true, "isPercent": true, "base": 1535, "value": 10 } },
            { "discountVat": { "name": "Voucher", "isDiscount": true, "value": 200 } },
            { "sumInCurrency": { "value": 2278 } }
          ]
        }
        """;

    [Fact]
    public async Task Biedronka_receipt_is_imported_with_discounts_and_voucher()
    {
        HttpResponseMessage response = await Upload(api.Editor, Receipt);
        ReceiptResponse receipt = await response.Read<ReceiptResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Biedronka", receipt.Store);
        Assert.Equal(new DateOnly(2026, 9, 28), receipt.PurchaseDate);
        Assert.Equal(
            [
                ("Mleko 2%", 2m, 6.98m, 1.48m, 5.50m),
                ("Chleb razowy", 1m, 4.99m, 0.40m, 4.59m),
                ("Ser gouda", 0.512m, 15.35m, 2.66m, 12.69m),
            ],
            receipt.Items.Select(item => (item.Item, item.Quantity, item.Amount, item.Discount, item.Total)));
        Assert.All(receipt.Items, item => Assert.Equal("Spożywcze", item.Category));
        Assert.Equal(22.78m, receipt.Total);
    }

    [Fact]
    public async Task Real_receipt_matches_what_was_paid_including_the_bottle_deposit()
    {
        string json = await File.ReadAllTextAsync(
            Path.Combine(AppContext.BaseDirectory, "Import", "biedronka-2026-09-11.json"),
            TestContext.Current.CancellationToken);

        ReceiptResponse receipt = await (await Upload(api.Editor, json)).Read<ReceiptResponse>();

        Assert.Equal(new DateOnly(2026, 9, 11), receipt.PurchaseDate);
        Assert.Equal(
            [
                ("ChipsyZCebLay s130g", 2m, 17.38m, 6.10m),
                ("TruskMrożMroźKra750g", 2m, 22.78m, 0m),
                ("KetBezCukKotlin420g", 1m, 6.99m, 0m),
                ("COCA COLA MIX 1l PET", 1m, 7.49m, 0m),
                ("KnoppersKokos40g", 2m, 5.90m, 0m),
                ("HummusSpicyGV160gN", 2m, 8.58m, 0m),
                ("Limonka szt", 2m, 3.98m, 0m),
                ("MLEK WYPAS 3,2 1L", 12m, 53.88m, 26.94m),
                ("But Plastik kaucja", 1m, 0.50m, 0m),
            ],
            receipt.Items.Select(item => (item.Item, item.Quantity, item.Amount, item.Discount)));
        Assert.Equal(94.44m, receipt.Total);
    }

    [Fact]
    public async Task Returned_deposit_lowers_the_whole_receipt()
    {
        HttpResponseMessage response = await Upload(api.Editor, """
            {
              "header": [ { "headerData": { "date": "2026-09-28T10:00:00Z" } } ],
              "body": [
                { "sellLine": { "name": "Woda A", "vatId": "A", "quantity": "1", "price": 300, "total": 300 } },
                { "sellLine": { "name": "Sok A", "vatId": "A", "quantity": "1", "price": 100, "total": 100 } },
                { "pack": { "name": "But Plastik kaucja", "price": 50, "quantity": "2", "total": 100, "isNegative": true } }
              ]
            }
            """);
        ReceiptResponse receipt = await response.Read<ReceiptResponse>();

        Assert.Equal([0.75m, 0.25m], receipt.Items.Select(item => item.Discount));
        Assert.Equal(3.00m, receipt.Total);
    }

    [Theory]
    [InlineData("", "Import.EmptyFile")]
    [InlineData("{ not json", "Import.InvalidJson")]
    [InlineData("""{ "body": [] }""", "Import.MissingDate")]
    [InlineData("""{ "header": [ { "headerData": { "date": "2026-09-28T10:00:00Z" } } ], "body": [] }""", "Import.NoItems")]
    [InlineData("""{ "header": [ { "headerData": { "date": "2026-09-28T10:00:00Z" } } ], "body": [ { "sellLine": { "name": "X", "quantity": "1", "price": 100, "isStorno": true } } ] }""", "Import.Storno")]
    [InlineData("""{ "header": [ { "headerData": { "date": "2026-09-28T10:00:00Z" } } ], "body": [ { "discountLine": { "value": 100 } } ] }""", "Import.DiscountWithoutItem")]
    [InlineData("""{ "header": [ { "headerData": { "date": "2026-09-28T10:00:00Z" } } ], "body": [ { "sellLine": { "name": "X", "quantity": "abc", "price": 100 } } ] }""", "Import.InvalidQuantity")]
    [InlineData("""{ "header": [ { "headerData": { "date": "2026-10-28T10:00:00Z" } } ], "body": [ { "sellLine": { "name": "X", "quantity": "1", "price": 100 } } ] }""", "PurchaseDate.InFuture")]
    [InlineData("""{ "header": [ { "headerData": { "date": "2026-09-28T10:00:00Z" } } ], "body": [ { "sellLine": { "name": "X", "quantity": "1", "price": 100 } }, { "discountLine": { "value": 150 } } ] }""", "LinePrice.DiscountExceedsAmount")]
    public async Task Problems_in_the_file_are_reported(string json, string code)
    {
        HttpResponseMessage response = await Upload(api.Editor, json);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(code, (await response.ErrorCodes())["file"]);
    }

    [Fact]
    public async Task Reader_cannot_import()
    {
        HttpResponseMessage response = await Upload(api.Reader, Receipt);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public void Purchase_date_is_the_local_date_of_the_purchase()
    {
        Result<ImportReceipt> result = BiedronkaReceiptParser.Parse(
            Receipt,
            ApiFixture.Today,
            TimeZoneInfo.FindSystemTimeZoneById("Europe/Warsaw"));

        // 22:30 UTC on the 28th is already 00:30 on the 29th in Poland.
        Assert.Equal(new DateOnly(2026, 9, 29), result.Match(receipt => receipt.PurchaseDate.Value, _ => default));
    }

    [Theory]
    [InlineData(1.00, new[] { 1.0, 1.0, 1.0 }, new[] { 0.34, 0.33, 0.33 })]
    [InlineData(0.05, new[] { 0.01, 10.0 }, new[] { 0.0, 0.05 })]
    [InlineData(3.00, new[] { 1.0, 2.0 }, new[] { 1.0, 2.0 })]
    [InlineData(0.00, new[] { 1.0, 2.0 }, new[] { 0.0, 0.0 })]
    public void Voucher_split_adds_up_to_the_voucher_in_whole_grosze(double total, double[] values, double[] expected)
    {
        IReadOnlyList<decimal> shares = Proportional.Split((decimal)total, [.. values.Select(value => (decimal)value)]);

        Assert.Equal(expected.Select(value => (decimal)value), shares);
        Assert.Equal((decimal)total, shares.Sum());
    }

    private static async Task<HttpResponseMessage> Upload(HttpClient client, string json)
    {
        using MultipartFormDataContent form = new();
        using ByteArrayContent file = new(Encoding.UTF8.GetBytes(json));
        file.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        form.Add(file, "file", "paragon.json");
        return await client.PostAsync(new Uri(Import, UriKind.Relative), form, TestContext.Current.CancellationToken);
    }
}
