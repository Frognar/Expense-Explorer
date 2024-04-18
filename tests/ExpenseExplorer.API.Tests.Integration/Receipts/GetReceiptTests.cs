namespace ExpenseExplorer.API.Tests.Integration.Receipts;

using System.Net;
using ExpenseExplorer.ReadModel;
using ExpenseExplorer.ReadModel.Models.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class GetReceiptTests(ReceiptApiFactory factory) : BaseIntegrationTest(factory), IAsyncLifetime
{
  public async Task InitializeAsync()
  {
    IServiceScope scope = ServiceScopeFactory.CreateScope();
    ExpenseExplorerContext dbContext = scope.ServiceProvider.GetRequiredService<ExpenseExplorerContext>();
    if (await dbContext.ReceiptHeaders.AnyAsync())
    {
      return;
    }

    DateOnly today = DateOnly.FromDateTime(DateTime.Today);
    dbContext.ReceiptHeaders.Add(new DbReceiptHeader("abc", "store", today, 0));
    await dbContext.SaveChangesAsync();
  }

  public Task DisposeAsync()
  {
    return Task.CompletedTask;
  }

  [Fact]
  public async Task CanGetReceipt()
  {
    Uri uri = new("api/receipts/abc", UriKind.Relative);
    HttpResponseMessage response = await Client.GetAsync(uri);
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }
}
