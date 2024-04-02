namespace ExpenseExplorer.API.Tests.Integration.Receipts;

using System.Net;
using System.Net.Http.Json;
using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Tests.Integration.Receipts.TestData;

public class AddPurchaseTests(ReceiptApiFactory factory) : BaseIntegrationTest(factory)
{
  [Theory]
  [ClassData(typeof(ValidAddPurchaseRequestData))]
  public async Task CanAddPurchaseToReceipt(object request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request);
    response.StatusCode.ShouldBeIn200Group();
  }

  [Theory]
  [ClassData(typeof(ValidAddPurchaseRequestData))]
  public async Task ContainsAddedPurchaseInResponse(object request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request);
    ReceiptResponse receipt = (await response.Content.ReadFromJsonAsync<ReceiptResponse>())!;
    receipt.Purchases.Count().Should().Be(1);
  }

  [Theory]
  [ClassData(typeof(InvalidAddPurchaseRequestData))]
  public async Task IsBadRequestWhenRequestIsInvalid(object request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request);
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Theory]
  [ClassData(typeof(ValidAddPurchaseRequestData))]
  public async Task IsNotFoundWhenReceiptIdIsInvalid(object request)
  {
    HttpResponseMessage response = await Client.PostAsJsonAsync("/api/receipts/invalid-id", request);
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  private async Task<HttpResponseMessage> SendWithValidReceiptId(object request)
  {
    object openNewRequest = new { storeName = "store", purchaseDate = DateOnly.MinValue };
    HttpResponseMessage newReceiptResponse = await Client.PostAsJsonAsync("/api/receipts", openNewRequest);
    ReceiptResponse receipt = (await newReceiptResponse.Content.ReadFromJsonAsync<ReceiptResponse>())!;
    return await Client.PostAsJsonAsync($"/api/receipts/{receipt.Id}", request);
  }
}
