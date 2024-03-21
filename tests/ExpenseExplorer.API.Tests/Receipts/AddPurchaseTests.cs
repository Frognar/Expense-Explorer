namespace ExpenseExplorer.API.Tests.Receipts;

using System.Net;
using System.Net.Http.Json;
using ExpenseExplorer.API.Contract;
using Microsoft.AspNetCore.Mvc.Testing;

public class AddPurchaseTests
{
  [Property(Arbitrary = [typeof(NonEmptyStringGenerator), typeof(DateOnlyGenerator), typeof(PositiveDecimalGenerator)])]
  public void ContainsDataGivenDuringConstruction(
    string productName,
    string productCategory,
    decimal quantity,
    decimal unitPrice,
    decimal? totalDiscount,
    string? description)
  {
    AddPurchaseRequest request = new(
      productName,
      productCategory,
      quantity,
      unitPrice,
      totalDiscount,
      description);

    request.Item.Should().Be(productName);
    request.Category.Should().Be(productCategory);
    request.Quantity.Should().Be(quantity);
    request.UnitPrice.Should().Be(unitPrice);
    request.TotalDiscount.Should().Be(totalDiscount);
    request.Description.Should().Be(description);
  }

  [Property(Arbitrary = [typeof(ValidAddPurchaseRequestGenerator)], MaxTest = 25)]
  public async Task CanAddPurchaseToReceipt(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request);
    response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
  }

  [Property(Arbitrary = [typeof(ValidAddPurchaseRequestGenerator)], MaxTest = 25)]
  public async Task ContainsAddedPurchaseInResponse(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request);
    OpenNewReceiptResponse receipt = (await response.Content.ReadFromJsonAsync<OpenNewReceiptResponse>())!;
    receipt.Purchases.Count().Should().Be(1);
  }

  [Property(Arbitrary = [typeof(ValidAddPurchaseRequestGenerator)], MaxTest = 25)]
  public async Task IsNotFoundWhenReceiptIdIsInvalid(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await Send("/api/receipts/invalid-id", request);
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Property(Arbitrary = [typeof(InvalidAddPurchaseRequestGenerator)], MaxTest = 25)]
  public async Task IsBadRequestWhenRequestIsInvalid(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request);
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  private static async Task<HttpResponseMessage> SendWithValidReceiptId(AddPurchaseRequest request)
  {
    OpenNewReceiptRequest openReceiptRequest = new("Store", TodayDateOnly);
    HttpResponseMessage response = await Send("/api/receipts", openReceiptRequest);
    OpenNewReceiptResponse receipt = (await response.Content.ReadFromJsonAsync<OpenNewReceiptResponse>())!;
    return await Send($"/api/receipts/{receipt.Id}", request);
  }

  private static async Task<HttpResponseMessage> Send(string uri, object request)
  {
    using WebApplicationFactory<Program> webAppFactory = new TestWebApplicationFactory();
    HttpClient client = webAppFactory.CreateClient();
    return await client.PostAsJsonAsync(uri, request);
  }
}
