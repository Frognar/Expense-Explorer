namespace ExpenseExplorer.API.Tests.Integration.Receipts;

using System.Net;
using System.Net.Http.Json;
using ExpenseExplorer.API.Contract;
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
    dbContext.ReceiptHeaders.Add(new DbReceiptHeader("abc", "store", today.AddDays(-1), 5));
    dbContext.ReceiptHeaders.Add(new DbReceiptHeader("bcd", "store 2", today, 1));
    await dbContext.SaveChangesAsync();
  }

  public Task DisposeAsync()
  {
    return Task.CompletedTask;
  }

  [Theory]
  [InlineData("abc")]
  [InlineData("bcd")]
  public async Task CanGetReceipt(string id)
  {
    HttpResponseMessage response = await Get(id);
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task NotFoundForUnknownReceipt()
  {
    HttpResponseMessage response = await Get("unknown");
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task ReturnsReceipt()
  {
    HttpResponseMessage response = await Get("abc");
    GetReceiptResponse receipt = (await response.Content.ReadFromJsonAsync<GetReceiptResponse>())!;
    receipt.Should().NotBeNull();
    receipt.Id.Should().Be("abc");
    receipt.Store.Should().Be("store");
    receipt.PurchaseDate.Should().Be(DateOnly.FromDateTime(DateTime.Today));
    receipt.Total.Should().Be(5);
  }

  private async Task<HttpResponseMessage> Get(string id)
  {
    Uri uri = new($"api/receipts/{id}", UriKind.Relative);
    return await Client.GetAsync(uri);
  }
}
