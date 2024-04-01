namespace ExpenseExplorer.API.Tests.Integration.Receipts;

using System.Net.Http.Json;
using ExpenseExplorer.API.Contract;

public class GetReceiptsTests(ReceiptApiFactory factory) : BaseIntegrationTest(factory), IAsyncLifetime
{
  public async Task InitializeAsync()
  {
    DateOnly today = DateOnly.FromDateTime(DateTime.Today);
    _ = await AsyncEnumerable.Range(1, 15)
      .SelectAwait(
        async i => await Client.PostAsJsonAsync(
          "api/receipts",
          new { storeName = $"store_{i}", purchaseDate = today }))
      .ToListAsync();
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
    Uri uri = new("api/receipts", UriKind.Relative);
    var result = await Client.GetAsync(uri);
    var response = await result.Content.ReadFromJsonAsync<GetReceiptsResponse>();
    response!.TotalCount.Should().Be(15);
  }
}
