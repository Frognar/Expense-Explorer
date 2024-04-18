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
  private static readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Today);

  private static readonly Dictionary<string, DbReceipt> _receipts = new()
  {
    { "abc", new DbReceipt("abc", "store", _today.AddDays(-1), 5) },
    { "bcd", new DbReceipt("bcd", "store 2", _today, 1) },
  };

  public async Task InitializeAsync()
  {
    IServiceScope scope = ServiceScopeFactory.CreateScope();
    ExpenseExplorerContext dbContext = scope.ServiceProvider.GetRequiredService<ExpenseExplorerContext>();
    if (await dbContext.Receipts.AnyAsync())
    {
      return;
    }

    await dbContext.AddRangeAsync(_receipts.Values);
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

  [Theory]
  [InlineData("abc")]
  [InlineData("bcd")]
  public async Task ReturnsReceipt(string id)
  {
    HttpResponseMessage response = await Get(id);
    GetReceiptResponse receipt = (await response.Content.ReadFromJsonAsync<GetReceiptResponse>())!;
    receipt.Should().NotBeNull();
    receipt.Id.Should().Be(_receipts[id].Id);
    receipt.Store.Should().Be(_receipts[id].Store);
    receipt.PurchaseDate.Should().Be(_receipts[id].PurchaseDate);
    receipt.Total.Should().Be(_receipts[id].Total);
  }

  private async Task<HttpResponseMessage> Get(string id)
  {
    Uri uri = new($"api/receipts/{id}", UriKind.Relative);
    return await Client.GetAsync(uri);
  }
}
