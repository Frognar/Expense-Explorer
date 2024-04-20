namespace ExpenseExplorer.API.Tests.Integration.Receipts;

using System.Net.Http.Json;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

public class UpdateReceiptTests(ReceiptApiFactory factory) : BaseIntegrationTest(factory), IAsyncLifetime
{
  private static string _receiptId = string.Empty;

  public async Task InitializeAsync()
  {
    IServiceScope scope = ServiceScopeFactory.CreateScope();
    IReceiptRepository repository = scope.ServiceProvider.GetRequiredService<IReceiptRepository>();
    DateOnly today = DateOnly.FromDateTime(DateTime.Today);
    if (string.IsNullOrEmpty(_receiptId))
    {
      Receipt receipt = Receipt.New(Store.Create("store"), PurchaseDate.Create(today, today), today);
      await repository.SaveAsync(receipt, default);
      _receiptId = receipt.Id.Value;
    }
  }

  public Task DisposeAsync()
  {
    return Task.CompletedTask;
  }

  [Fact]
  public async Task CanUpdateReceipt()
  {
    Uri uri = new($"/api/receipts/{_receiptId}");
    HttpResponseMessage response = await Client.PatchAsJsonAsync(uri, new { });
    response.StatusCode.ShouldBeIn200Group();
  }
}
