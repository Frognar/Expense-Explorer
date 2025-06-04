using System.Data;
using Dapper;

namespace ExpenseExplorer.Infrastructure.Database;

internal sealed class DbInitializer(IDbConnectionFactory connectionFactory)
{
    public async Task InitializeAsync()
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(CancellationToken.None);
        await connection.ExecuteAsync($"""
                                       create table if not exists receipts (
                                           id UUID primary key,
                                           store text not null,
                                           purchase_date date not null);
                                       """);

        await connection.ExecuteAsync("""
                                      create table if not exists receipt_items (
                                          id UUID primary key,
                                          receipt_id UUID not null references receipts(id),
                                          item text not null,
                                          category text not null,
                                          unit_price decimal(15, 4) not null,
                                          quantity decimal(12, 4) not null,
                                          discount decimal(15, 2),
                                          description text)
                                      """);
    }
}