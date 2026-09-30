using System.Net;
using ExpenseExplorer.Contracts.Receipts;
using Npgsql;

namespace ExpenseExplorer.Api.Tests;

/// <summary>The first start of v2 on the Pi meets tables created by the previous version.</summary>
public class LegacyDatabaseTests(ApiFixture api)
{
    [Fact]
    public async Task Existing_receipts_are_kept_and_readable_after_migration()
    {
        string connectionString = await api.CreateDatabaseAsync("legacy_app");
        Guid receiptId = Guid.NewGuid();
        await using (NpgsqlConnection connection = new(connectionString))
        {
            await connection.OpenAsync(TestContext.Current.CancellationToken);
            await using NpgsqlCommand command = new(
                $"""
                create table receipts (
                    id UUID primary key,
                    store text not null,
                    purchase_date date not null);
                create table receipt_items (
                    id UUID primary key,
                    receipt_id UUID not null references receipts(id),
                    item text not null,
                    category text not null,
                    unit_price decimal(15, 4) not null,
                    quantity decimal(12, 4) not null,
                    discount decimal(15, 2),
                    description text);
                insert into receipts values ('{receiptId}', 'Biedronka', '2025-01-15');
                insert into receipt_items values
                    ('{Guid.NewGuid()}', '{receiptId}', 'Mleko', 'Spożywcze', 3.49, 2, null, null),
                    ('{Guid.NewGuid()}', '{receiptId}', 'Chleb', 'Spożywcze', 5.99, 1, 1.00, 'promocja');
                """,
                connection);
            await command.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);
        }

        await using ApiFactory app = new(connectionString);
        HttpResponseMessage response = await app.CreateClient().Get($"/api/v1/receipts/{receiptId}");
        ReceiptResponse receipt = await response.Read<ReceiptResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Biedronka", receipt.Store);
        Assert.Equal(2, receipt.Items.Count);
        Assert.Equal(11.97m, receipt.Total);
    }
}
