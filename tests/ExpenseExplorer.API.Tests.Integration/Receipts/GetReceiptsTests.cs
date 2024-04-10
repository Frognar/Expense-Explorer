namespace ExpenseExplorer.API.Tests.Integration.Receipts;

using System.Net.Http.Json;
using CommandHub.Commands;
using ExpenseExplorer.API.Contract;
using ExpenseExplorer.ReadModel;
using ExpenseExplorer.ReadModel.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class GetReceiptsTests(ReceiptApiFactory factory) : BaseIntegrationTest(factory), IAsyncLifetime
{
  public async Task InitializeAsync()
  {
    DateOnly today = DateOnly.FromDateTime(DateTime.Today);
    var scope = ServiceScopeFactory.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ExpenseExplorerContext>();
    if (await dbContext.ReceiptHeaders.AnyAsync())
    {
      return;
    }

    var createReceiptCommandHandler
      = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateReceiptCommand, Unit>>();

    _ = await AsyncEnumerable.Range(1, 15)
      .SelectAwait(
        async i => await createReceiptCommandHandler.HandleAsync(
          new CreateReceiptCommand(Guid.NewGuid().ToString("N"), $"store_{i}", today)))
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
    var response = await GetReceipts();
    response.TotalCount.Should().Be(15);
  }

  [Fact]
  public async Task ReturnsPageOfReceipts()
  {
    var response = await GetReceipts();
    response.Receipts.Should().HaveCount(10);
  }

  private async Task<GetReceiptsResponse> GetReceipts()
  {
    Uri uri = new("api/receipts", UriKind.Relative);
    var result = await Client.GetAsync(uri);
    return (await result.Content.ReadFromJsonAsync<GetReceiptsResponse>())!;
  }
}
