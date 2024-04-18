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
    HttpResponseMessage response = await Get("abc");
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task NotFoundForUnknownReceipt()
  {
    HttpResponseMessage response = await Get("unknown");
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  private async Task<HttpResponseMessage> Get(string id)
  {
    Uri uri = new($"api/receipts/{id}", UriKind.Relative);
    return await Client.GetAsync(uri);
  }
}
