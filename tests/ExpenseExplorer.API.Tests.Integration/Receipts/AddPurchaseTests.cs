namespace ExpenseExplorer.API.Tests.Integration.Receipts;

public class AddPurchaseTests(ReceiptApiFactory factory) : BaseIntegrationTest(factory), IAsyncLifetime
{
  private string _receiptId = string.Empty;

  public async Task InitializeAsync()
  {
    IServiceScope scope = ServiceScopeFactory.CreateScope();
    IReceiptRepository repository = scope.ServiceProvider.GetRequiredService<IReceiptRepository>();
    Receipt receipt = TestFactory.Receipt("store", new DateOnly(2000, 1, 1));
    await repository.SaveAsync(receipt, default);
    _receiptId = receipt.Id.Value;
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
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    response.Headers.Location.Should().NotBeNull();
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

  [Theory]
  [ClassData(typeof(ValidAddPurchaseRequestData))]
  public async Task ContainsAddedPurchaseInResponse(object request)
  {
    HttpResponseMessage response = await Post(_receiptId, request);
    AddPurchaseResponse receipt = (await response.Content.ReadFromJsonAsync<AddPurchaseResponse>())!;
    receipt.Purchases.Count().Should().Be(1);
  }

  private async Task<HttpResponseMessage> Post(string receiptId, object request)
  {
    return await Client.PostAsJsonAsync($"/api/receipts/{receiptId}/purchases", request);
  }
}
