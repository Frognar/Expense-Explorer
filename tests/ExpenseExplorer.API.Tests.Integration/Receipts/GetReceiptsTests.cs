namespace ExpenseExplorer.API.Tests.Integration.Receipts;

using System.Net.Http.Json;
using ExpenseExplorer.API.Contract;
using ExpenseExplorer.ReadModel;
using ExpenseExplorer.ReadModel.Models.Persistence;
using ExpenseExplorer.ReadModel.Queries;
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
    HttpResponseMessage result = await Get();
    result.StatusCode.ShouldBeIn200Group();
  }

  [Fact]
  public async Task ReturnsTotalCountInResponse()
  {
    GetReceiptsResponse response = await GetReceipts();
    response.TotalCount.Should().Be(_totalReceipts);
  }

  [Property]
  public async Task ReturnsPageSizeInResponse(int pageSize)
  {
    int expectedPageSize = pageSize < 1
      ? GetReceiptsQuery.DefaultPageSize
      : Math.Min(pageSize, GetReceiptsQuery.MaxPageSize);

    GetReceiptsResponse response = await GetReceipts($"?pageSize={pageSize}");
    response.PageSize.Should().Be(expectedPageSize);
  }

  [Fact]
  public async Task ReturnsPageOfReceipts()
  {
    GetReceiptsResponse response = await GetReceipts();
    response.Receipts.Should().HaveCount(GetReceiptsQuery.DefaultPageSize);
  }

  [Theory]
  [InlineData(1)]
  [InlineData(5)]
  [InlineData(25)]
  public async Task CanDefinePageSize(int pageSize)
  {
    GetReceiptsResponse response = await GetReceipts($"?pageSize={pageSize}");
    response.Receipts.Should().HaveCount(pageSize);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public async Task ReturnsDefaultPageSizeWhenInvalid(int pageSize)
  {
    GetReceiptsResponse response = await GetReceipts($"?pageSize={pageSize}");
    response.Receipts.Should().HaveCount(GetReceiptsQuery.DefaultPageSize);
  }

  [Fact]
  public async Task ReturnsMaxPageSize()
  {
    GetReceiptsResponse response = await GetReceipts($"?pageSize={GetReceiptsQuery.MaxPageSize + 1}");
    response.Receipts.Should().HaveCount(GetReceiptsQuery.MaxPageSize);
  }

  private async Task<GetReceiptsResponse> GetReceipts(string parameters = "")
  {
    HttpResponseMessage result = await Get(parameters);
    return (await result.Content.ReadFromJsonAsync<GetReceiptsResponse>())!;
  }

  private async Task<HttpResponseMessage> Get(string parameters = "")
  {
    Uri uri = new("api/receipts" + parameters, UriKind.Relative);
    return await Client.GetAsync(uri);
  }
}
