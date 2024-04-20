namespace ExpenseExplorer.API.Tests.Integration.Receipts;

using System.Net;
using System.Net.Http.Json;
using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Tests.Integration.Receipts.TestData;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

public class AddPurchaseTests(ReceiptApiFactory factory) : BaseIntegrationTest(factory), IAsyncLifetime
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

  [Theory]
  [ClassData(typeof(ValidAddPurchaseRequestData))]
  public async Task CanAddPurchaseToReceipt(object request)
  {
    HttpResponseMessage response = await Post(_receiptId, request);
    response.StatusCode.ShouldBeIn200Group();
  }

  [Theory]
  [ClassData(typeof(ValidAddPurchaseRequestData))]
  public async Task ContainsAddedPurchaseInResponse(object request)
  {
    HttpResponseMessage response = await Post(_receiptId, request);
    AddPurchaseResponse receipt = (await response.Content.ReadFromJsonAsync<AddPurchaseResponse>())!;
    receipt.Purchases.Count().Should().BeGreaterThan(0);
  }

  [Theory]
  [ClassData(typeof(InvalidAddPurchaseRequestData))]
  public async Task IsBadRequestWhenRequestIsInvalid(object request)
  {
    HttpResponseMessage response = await Post(_receiptId, request);
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Theory]
  [ClassData(typeof(ValidAddPurchaseRequestData))]
  public async Task IsNotFoundWhenReceiptIdIsInvalid(object request)
  {
    HttpResponseMessage response = await Post("invalid-id", request);
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  private async Task<HttpResponseMessage> Post(string receiptId, object request)
  {
    return await Client.PostAsJsonAsync($"/api/receipts/{receiptId}/purchases", request);
  }
}
