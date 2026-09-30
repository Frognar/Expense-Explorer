using System.Net;
using System.Text;
using ExpenseExplorer.Contracts.Receipts;

namespace ExpenseExplorer.Api.Tests;

public class ExportApiTests(ApiFixture api)
{
    [Fact]
    public async Task Receipt_exports_as_csv_that_spreadsheets_read_safely()
    {
        ReceiptResponse receipt = await api.Editor.ReceiptAsync(
            "Żabka, Rynek",
            new DateOnly(2026, 9, 1),
            new ReceiptItemRequest("Woda", "Media", 17.3m, 123.45m, 3.45m, "=cmd|' /C calc'!A0"),
            new ReceiptItemRequest("Chleb \"razowy\"", "Pieczywo", 1m, 4.99m, null, null));

        HttpResponseMessage response = await api.Reader.Get($"/api/v1/receipts/{receipt.Id}/export.csv");
        byte[] bytes = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/csv", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("Żabka--Rynek-2026-09-01.csv", response.Content.Headers.ContentDisposition?.FileNameStar);
        Assert.Equal(Encoding.UTF8.GetPreamble(), bytes[..3]);
        Assert.Equal(
            """"
            Store,PurchaseDate,Total
            "Żabka, Rynek",2026-09-01,124.99

            Item,Category,Quantity,UnitPrice,Amount,Discount,Total,Description
            Woda,Media,17.3,7.1358,123.45,3.45,120.00,'=cmd|' /C calc'!A0
            "Chleb ""razowy""",Pieczywo,1,4.99,4.99,0.00,4.99,

            """".ReplaceLineEndings("\r\n"),
            Encoding.UTF8.GetString(bytes[3..]));
    }

    [Fact]
    public async Task Exporting_an_unknown_receipt_is_not_found()
    {
        HttpResponseMessage response = await api.Reader.Get($"/api/v1/receipts/{Guid.NewGuid()}/export.csv");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
