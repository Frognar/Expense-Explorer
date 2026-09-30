using System.Net;
using ExpenseExplorer.Contracts.Receipts;
using Npgsql;

namespace ExpenseExplorer.Api.Tests;

/// <summary>The first start of v2 on the Pi meets tables created by the previous version.</summary>
public class LegacyDatabaseTests(ApiFixture api)
{
    [Fact]
    public async Task Data_of_the_previous_version_is_copied_and_the_old_tables_are_kept()
    {
        string connectionString = await api.CreateDatabaseAsync("legacy_app");
        Guid receiptId = Guid.NewGuid();
        await ExecuteAsync(connectionString, $"""
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
            insert into receipts values ('{receiptId}', ' Biedronka ', '2025-01-15');
            insert into receipt_items values
                ('{Guid.NewGuid()}', '{receiptId}', 'Mleko', 'Spożywcze', 3.49, 2, null, null),
                ('{Guid.NewGuid()}', '{receiptId}', 'Chleb', 'Spożywcze', 5.99, 1, 1.00, 'promocja'),
                ('{Guid.NewGuid()}', '{receiptId}', 'Woda', 'Media', 7.1358, 17.3, null, '  ');
            """);

        await using ApiFactory app = new(connectionString);
        HttpResponseMessage response = await app.CreateClient().Get($"/api/v1/receipts/{receiptId}");
        ReceiptResponse receipt = await response.Read<ReceiptResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Biedronka", receipt.Store);
        Assert.Equal(
            [("Chleb", 5.99m, 4.99m, "promocja"), ("Mleko", 6.98m, 6.98m, null), ("Woda", 123.45m, 123.45m, null)],
            receipt.Items
                .Select(item => (item.Item, item.Amount, item.Total, item.Description))
                .OrderBy(item => item.Item));
        Assert.Equal(135.42m, receipt.Total);
        Assert.Equal(3L, await ScalarAsync(connectionString, "select count(*) from public.receipt_items"));
    }

    private static async Task ExecuteAsync(string connectionString, string sql)
    {
        await using NpgsqlConnection connection = new(connectionString);
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        await using NpgsqlCommand command = new(sql, connection);
        await command.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);
    }

    private static async Task<object?> ScalarAsync(string connectionString, string sql)
    {
        await using NpgsqlConnection connection = new(connectionString);
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        await using NpgsqlCommand command = new(sql, connection);
        return await command.ExecuteScalarAsync(TestContext.Current.CancellationToken);
    }
}
