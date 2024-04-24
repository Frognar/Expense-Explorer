namespace ExpenseExplorer.API.Tests.Integration.Receipts;

using System.Net;
using System.Net.Http.Json;
using ExpenseExplorer.API.Contract;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

public class UpdateReceiptTests(ReceiptApiFactory factory) : BaseIntegrationTest(factory), IAsyncLifetime
{
  private readonly string _storeName = "store";
  private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Today);
  private string _receiptId = string.Empty;

  public async Task InitializeAsync()
  {
    IServiceScope scope = ServiceScopeFactory.CreateScope();
    IReceiptRepository repository = scope.ServiceProvider.GetRequiredService<IReceiptRepository>();
    Receipt receipt = Receipt.New(Store.Create(_storeName), PurchaseDate.Create(_today, _today), _today);
    await repository.SaveAsync(receipt, default);
    _receiptId = receipt.Id.Value;
  }

  public Task DisposeAsync()
  {
    return Task.CompletedTask;
  }

  [Fact]
  public async Task CanUpdateReceipt()
  {
    HttpResponseMessage message = await Patch(_receiptId, new { });
    message.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task CanUpdateReceiptWithNewStoreName()
  {
    UpdateReceiptResponse response = await PatchReceipt(storeName: "new store");
    response.StoreName.Should().Be("new store");
    response.Version.Should().Be(1);
  }

  [Fact]
  public async Task CanUpdateReceiptWithNewPurchaseDate()
  {
    UpdateReceiptResponse response = await PatchReceipt(purchaseDate: "2022-01-01");
    response.PurchaseDate.Should().Be(new DateOnly(2022, 1, 1));
    response.Version.Should().Be(1);
  }

  [Fact]
  public async Task CanUpdateBothStoreNameAndPurchaseDate()
  {
    UpdateReceiptResponse response = await PatchReceipt("new store", "2022-01-01");
    response.StoreName.Should().Be("new store");
    response.PurchaseDate.Should().Be(new DateOnly(2022, 1, 1));
    response.Version.Should().Be(2);
  }

  [Fact]
  public async Task DontUpdateReceiptWhenNoChanges()
  {
    UpdateReceiptResponse response = await PatchReceipt();
    response.StoreName.Should().Be(_storeName);
    response.PurchaseDate.Should().Be(_today);
    response.Version.Should().Be(0);
  }

  private async Task<UpdateReceiptResponse> PatchReceipt(string? storeName = null, string? purchaseDate = null)
  {
    HttpResponseMessage message = await Patch(_receiptId, new { storeName, purchaseDate });
    return (await message.Content.ReadFromJsonAsync<UpdateReceiptResponse>())!;
  }

  private async Task<HttpResponseMessage> Patch(string receiptId, object request)
  {
    Uri uri = new($"/api/receipts/{receiptId}");
    return await Client.PatchAsJsonAsync(uri, request);
  }
}
