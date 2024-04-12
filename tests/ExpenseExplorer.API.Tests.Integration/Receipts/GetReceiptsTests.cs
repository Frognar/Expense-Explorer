namespace ExpenseExplorer.API.Tests.Integration.Receipts;

using System.Net.Http.Json;
using ExpenseExplorer.API.Contract;
using ExpenseExplorer.ReadModel;
using ExpenseExplorer.ReadModel.Models.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class GetReceiptsTests(ReceiptApiFactory factory) : BaseIntegrationTest(factory), IAsyncLifetime
{
  private const int _totalReceipts = 51;

  public async Task InitializeAsync()
  {
    DateOnly today = DateOnly.FromDateTime(DateTime.Today);
    var scope = ServiceScopeFactory.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ExpenseExplorerContext>();
    if (await dbContext.ReceiptHeaders.AnyAsync())
    {
      return;
    }

    DbReceiptHeader CreateReceiptHeader(int i) => new(Guid.NewGuid().ToString("N"), $"store_{i}", today, 0);
    IEnumerable<DbReceiptHeader> receiptHeaders = Enumerable.Range(1, _totalReceipts).Select(CreateReceiptHeader);
    await dbContext.ReceiptHeaders.AddRangeAsync(receiptHeaders);
    await dbContext.SaveChangesAsync();
  }

  public Task DisposeAsync()
  {
    return Task.CompletedTask;
  }

  [Fact]
  public async Task CanGetReceipts()
  {
    Uri uri = new("api/receipts", UriKind.Relative);
    var result = await Client.GetAsync(uri);
    result.StatusCode.ShouldBeIn200Group();
  }

  [Fact]
  public async Task ReturnsTotalCountInResponse()
  {
    var response = await GetReceipts();
    response.TotalCount.Should().Be(_totalReceipts);
  }

  [Fact]
  public async Task ReturnsPageOfReceipts()
  {
    var response = await GetReceipts();
    response.Receipts.Should().HaveCount(10);
  }

  [Fact]
  public async Task CanDefinePageSize()
  {
    var response = await GetReceipts("?pageSize=5");
    response.Receipts.Should().HaveCount(5);
  }

  private async Task<GetReceiptsResponse> GetReceipts(string parameters = "")
  {
    Uri uri = new("api/receipts" + parameters, UriKind.Relative);
    var result = await Client.GetAsync(uri);
    return (await result.Content.ReadFromJsonAsync<GetReceiptsResponse>())!;
  }
}
