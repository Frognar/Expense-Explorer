using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using ExpenseExplorer.Api.Receipts.Import.Photo;
using ExpenseExplorer.Contracts.Receipts;
using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Api.Tests;

public class PhotoImportApiTests(ApiFixture api)
{
    private const string Import = "/api/v1/receipts/import/photo";

    /// <summary>What the OCR service read from a photo of a crumpled receipt from Dino.</summary>
    private static readonly string DinoPhoto = File.ReadAllText(
        Path.Combine(AppContext.BaseDirectory, "Import", "dino-2026-10-01.ocr.json"));

    [Fact]
    public async Task Photo_of_a_receipt_is_imported_line_by_line()
    {
        HttpResponseMessage response = await Upload(api.Editor, DinoPhoto);
        PhotoImportResponse imported = await response.Read<PhotoImportResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Dino", imported.Receipt.Store);
        Assert.Equal(22, imported.Receipt.Items.Count);
        Assert.Equal(150.60m, imported.Receipt.Total);
        Assert.Equal(150.60m, imported.PrintedTotal);
        Assert.Empty(imported.SkippedLines);
    }

    [Fact]
    public async Task Date_after_today_is_not_trusted_and_today_is_used()
    {
        // The receipt is dated 2026-10-01, after the test clock's today.
        PhotoImportResponse imported = await (await Upload(api.Editor, DinoPhoto)).Read<PhotoImportResponse>();

        Assert.False(imported.PurchaseDateFound);
        Assert.Equal(ApiFixture.Today, imported.Receipt.PurchaseDate);
    }

    [Fact]
    public async Task Photo_without_receipt_lines_is_refused()
    {
        HttpResponseMessage response = await Upload(api.Editor, """{ "lines": [] }""");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Import.NothingRead", (await response.ErrorCodes())["file"]);
    }

    [Fact]
    public async Task Reader_cannot_import_a_photo()
    {
        HttpResponseMessage response = await Upload(api.Reader, DinoPhoto);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public void Every_line_of_the_Dino_receipt_is_read_with_its_discount()
    {
        PaperReceipt receipt = ParseDino(today: new DateOnly(2026, 10, 1));

        Assert.Equal("Dino", receipt.Receipt.Store.Value);
        Assert.Equal(new DateOnly(2026, 10, 1), receipt.Receipt.PurchaseDate.Value);
        Assert.True(receipt.PurchaseDateFound);
        Assert.Equal(
            [
                ("SER ZANETTI 100G", 1m, 7.49m, 0m),
                ("SER KOZI NAT 160G", 1m, 10.65m, 0m),
                ("BURACZKI ZASMAZ 83", 2m, 11.98m, 0m),
                ("NAPOJ OWSI PU 1L", 1m, 4.46m, 0m),
                ("MASLANKA NATUR 1L", 1m, 3.99m, 0m),
                ("SEREK WIEJ 150G", 1m, 2.59m, 0m),
                ("MAKARO ZE SZPIN 40", 1m, 4.99m, 0m),
                ("SMIETANKA 330ML", 1m, 5.49m, 0m),
                ("SALATA MASL SZT", 1m, 3.99m, 0m),
                ("POR SZT.", 1m, 3.99m, 0m),
                ("KABANOSY 120G", 1m, 4.99m, 2.50m),
                ("KABANOSY DRO 120G", 1m, 4.99m, 2.50m),
                ("JAJA WOLNY MX10", 1m, 10.99m, 0m),
                ("FRANKFURTERKI", 0.380m, 11.02m, 0m),
                ("JABLKO KG", 0.792m, 1.58m, 0m),
                ("MARCHEW KG", 0.618m, 2.47m, 0m),
                ("CEBULA KG", 0.360m, 1.26m, 0m),
                ("WATROBA Z KURCZ", 0.548m, 5.47m, 0m),
                ("SERCA Z KURCZA SW", 0.542m, 5.96m, 0m),
                ("ZOLADKI Z KURCZ", 0.516m, 5.15m, 0m),
                ("SZYNKA WIEP B/K", 1.034m, 13.43m, 0m),
                ("FILET Z PIER IND", 0.956m, 28.67m, 0m),
            ],
            receipt.Receipt.Purchases.Select(purchase => (
                purchase.Item.Value,
                purchase.Price.Quantity.Value,
                purchase.Price.Amount.Value,
                purchase.Price.Discount.Value)));
        Assert.All(receipt.Receipt.Purchases, purchase => Assert.Equal("Spożywcze", purchase.Category.Value));
        Assert.Equal(150.60m, receipt.PrintedTotal);
    }

    [Fact]
    public void Pieces_at_the_same_height_of_a_tilted_photo_make_one_line()
    {
        // The photo is turned so that text rises 10 px for every 100 px to the right: the price of
        // the milk ends up level with the bread.
        OcrText[] texts =
        [
            Box("1 x4,99 4,99C", left: 600, line: 200, width: 300),
            Box("CHLEB C", left: 0, line: 200, width: 200),
            Box("MLEKO C", left: 0, line: 260, width: 200),
            Box("1 x3,49 3,49C", left: 600, line: 260, width: 300),
        ];

        Assert.Equal(["CHLEB C 1 x4,99 4,99C", "MLEKO C 1 x3,49 3,49C"], PrintedLines.Of(texts));
    }

    [Fact]
    public void Name_wrapped_onto_its_own_line_is_joined_with_the_price_below()
    {
        PaperReceipt receipt = Parse("PARAGON FISKALNY", "MAKARON PELNOZIARNISTY SPAGHETTI", "1 x4,99 4,99C", "SUMA PLN 4,99");

        Assert.Equal("MAKARON PELNOZIARNISTY SPAGHETTI", Assert.Single(receipt.Receipt.Purchases).Item.Value);
        Assert.Empty(receipt.SkippedLines);
    }

    [Fact]
    public void Unreadable_lines_are_skipped_and_reported()
    {
        PaperReceipt receipt = Parse(
            "PARAGON FISKALNY",
            "CHLEB C 1 x4,99 4,99C",
            "MLEKO C 1 ?3,4 3,49C",
            "OPUST -9,99C",
            "SUMA PLN 8,48");

        Assert.Equal("CHLEB", Assert.Single(receipt.Receipt.Purchases).Item.Value);
        Assert.Equal(["MLEKO C 1 ?3,4 3,49C", "OPUST -9,99C"], receipt.SkippedLines);
        Assert.Equal(8.48m, receipt.PrintedTotal);
    }

    [Fact]
    public void Receipt_is_read_when_its_title_was_not_recognized()
    {
        PaperReceipt receipt = Parse("DINO POLSKA S.A.", "NIP 6211766191", "FRANKFURTERKI C 0,.380 ×28.99 11,02C", "SUMA PLN 11,02");

        Assert.Equal("Dino", receipt.Receipt.Store.Value);
        Purchase frankfurters = Assert.Single(receipt.Receipt.Purchases);
        Assert.Equal(0.380m, frankfurters.Price.Quantity.Value);
        Assert.Equal(11.02m, frankfurters.Price.Amount.Value);
        Assert.Empty(receipt.SkippedLines);
    }

    [Fact]
    public void Sum_of_discounts_is_not_taken_for_a_discount_on_the_last_item()
    {
        // "Ł" of "ŁĄCZNIE" misread as "K", and a stray letter inside "SUMA PLN".
        PaperReceipt receipt = Parse(
            "PARAGON FISKALNY",
            "KABANOSY C 1 x4,99 4,99C",
            "OPUST KABANOSY C -2,50C",
            "FILET C 0,956 x29,99 28,67C",
            "OPUSTY KACZNIE -2,50",
            "SUMA A PLN 31,16");

        Assert.Equal([2.50m, 0m], receipt.Receipt.Purchases.Select(purchase => purchase.Price.Discount.Value));
        Assert.Equal(31.16m, receipt.PrintedTotal);
    }

    [Fact]
    public void Missing_amount_is_price_times_quantity()
    {
        PaperReceipt receipt = Parse("PARAGON FISKALNY", "JABLKA KG C 1,255 x3,99", "SUMA PLN 5,01");

        Assert.Equal(5.01m, Assert.Single(receipt.Receipt.Purchases).Price.Amount.Value);
    }

    [Fact]
    public void Date_is_read_in_either_order_and_today_is_used_without_one()
    {
        DateOnly today = new(2026, 10, 1);

        Assert.Equal(new DateOnly(2026, 9, 30), Parse(today, "PARAGON FISKALNY", "A C 1 x1,00 1,00C", "30.09.2026 18:05").Receipt.PurchaseDate.Value);
        Assert.Equal(new DateOnly(2026, 9, 30), Parse(today, "2026-09-30 18:05", "PARAGON FISKALNY", "A C 1 x1,00 1,00C").Receipt.PurchaseDate.Value);

        PaperReceipt undated = Parse(today, "PARAGON FISKALNY", "A C 1 x1,00 1,00C");
        Assert.False(undated.PurchaseDateFound);
        Assert.Equal(today, undated.Receipt.PurchaseDate.Value);
    }

    [Theory]
    [InlineData("JERONIMO MARTINS POLSKA S.A.", "Biedronka")]
    [InlineData("Lidl sp. z o.o. sp. k.", "Lidl")]
    [InlineData("PIEKARNIA KOWALSKI SP. Z O.O.", "PIEKARNIA KOWALSKI")]
    public void Store_is_the_chain_or_the_company_on_top(string company, string store)
    {
        Assert.Equal(store, Parse(company, "ul. Polna 1", "PARAGON FISKALNY", "A C 1 x1,00 1,00C").Receipt.Store.Value);
    }

    private static PaperReceipt ParseDino(DateOnly today)
    {
        OcrResponse ocr = JsonSerializer.Deserialize<OcrResponse>(DinoPhoto, JsonSerializerOptions.Web)!;
        return Value(PaperReceiptParser.Parse(PrintedLines.Of(ocr.Texts()), today));
    }

    private static PaperReceipt Parse(params string[] lines) => Parse(ApiFixture.Today, lines);

    private static PaperReceipt Parse(DateOnly today, params string[] lines) => Value(PaperReceiptParser.Parse(lines, today));

    private static T Value<T>(Result<T> result) =>
        result.Match(value => value, errors => throw new InvalidOperationException(string.Join(", ", errors.Select(error => error.Code))));

    /// <summary>A piece of text on a photo tilted by 10%; <paramref name="line"/> is where its line starts at the left edge.</summary>
    private static OcrText Box(string text, double left, double line, double width)
    {
        const double rise = 0.1;
        const double height = 40;
        double right = left + width;
        return new OcrText(
            text,
            new OcrPoint(left, line - (rise * left)),
            new OcrPoint(right, line - (rise * right)),
            new OcrPoint(right, line - (rise * right) + height),
            new OcrPoint(left, line - (rise * left) + height));
    }

    private static async Task<HttpResponseMessage> Upload(HttpClient client, string ocrResponse)
    {
        using MultipartFormDataContent form = new();
        using ByteArrayContent file = new(System.Text.Encoding.UTF8.GetBytes(ocrResponse));
        file.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        form.Add(file, "file", "paragon.jpg");
        return await client.PostAsync(new Uri(Import, UriKind.Relative), form, TestContext.Current.CancellationToken);
    }
}

/// <summary>Stands in for the OCR service: the uploaded "photo" is what the service would have answered.</summary>
internal sealed class UploadedOcrResponse : IReceiptOcr
{
    public async Task<Result<IReadOnlyList<OcrText>>> ReadAsync(Stream photo, string contentType, CancellationToken cancellationToken)
    {
        OcrResponse? response = await JsonSerializer.DeserializeAsync<OcrResponse>(photo, JsonSerializerOptions.Web, cancellationToken);
        return Result.Success(response?.Texts() ?? []);
    }
}
