namespace ExpenseExplorer.API.Tests.Integration.Receipts;

using System.Diagnostics;
using System.Net.Http.Json;
using ExpenseExplorer.API.Contract;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

public class UpdateReceiptTests(ReceiptApiFactory factory) : BaseIntegrationTest(factory), IAsyncLifetime
{
  private static string _receiptId = string.Empty;
  private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Today);

  public async Task InitializeAsync()
  {
    IServiceScope scope = ServiceScopeFactory.CreateScope();
    IReceiptRepository repository = scope.ServiceProvider.GetRequiredService<IReceiptRepository>();
    if (string.IsNullOrEmpty(_receiptId))
    {
      Receipt receipt = Receipt.New(Store.Create("store"), PurchaseDate.Create(_today, _today), _today);
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
    HttpResponseMessage message = await Patch(_receiptId, new { });
    message.StatusCode.ShouldBeIn200Group();
  }

  [Fact]
  public async Task ContainsUpdatedReceiptInResponse()
  {
    UpdateReceiptResponse response = await PatchReceipt(_receiptId, new { });
    response.Should().NotBeNull();
  }

  [Fact]
  public async Task CanUpdateReceiptWithNewStoreName()
  {
    UpdateReceiptResponse response = await PatchReceipt(_receiptId, new { storeName = "new store" });
    response.Id.Should().Be(_receiptId);
    response.StoreName.Should().Be("new store");
    response.PurchaseDate.Should().Be(_today);
    response.Version.Should().Be(1);
  }

  [Fact]
  public async Task DontUpdateReceiptWhenNoChanges()
  {
    Receipt receipt = await GetReceiptFromDb();

    UpdateReceiptResponse response = await PatchReceipt(_receiptId, new { });

    response.StoreName.Should().Be(receipt.Store.Name);
    response.PurchaseDate.Should().Be(receipt.PurchaseDate.Date);
    response.Version.Should().Be(receipt.Version.Value);
  }

  private async Task<Receipt> GetReceiptFromDb()
  {
    IServiceScope scope = ServiceScopeFactory.CreateScope();
    IReceiptRepository repository = scope.ServiceProvider.GetRequiredService<IReceiptRepository>();
    return (await repository.GetAsync(Id.Create(_receiptId), default))
      .Match(_ => throw new UnreachableException(), r => r);
  }

  private async Task<UpdateReceiptResponse> PatchReceipt(string receiptId, object request)
  {
    HttpResponseMessage message = await Patch(receiptId, request);
    return (await message.Content.ReadFromJsonAsync<UpdateReceiptResponse>())!;
  }

  private async Task<HttpResponseMessage> Patch(string receiptId, object request)
  {
    Uri uri = new($"/api/receipts/{receiptId}");
    return await Client.PatchAsJsonAsync(uri, request);
  }
}
