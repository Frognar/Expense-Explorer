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

    request.ProductName.Should().Be(productName);
    request.ProductCategory.Should().Be(productCategory);
    request.Quantity.Should().Be(quantity);
    request.UnitPrice.Should().Be(unitPrice);
    request.TotalDiscount.Should().Be(totalDiscount);
    request.Description.Should().Be(description);
  }

  [Property(Arbitrary = [typeof(ValidAddPurchaseRequestGenerator)], MaxTest = 10)]
  public async Task CanAddReceipt(AddPurchaseRequest request)
  {
    string validReceiptId = await GetValidReceiptId().ConfigureAwait(false);

    HttpResponseMessage response = await Send(validReceiptId, request).ConfigureAwait(false);

    response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
  }

  [Property(Arbitrary = [typeof(AddPurchaseRequestWithInvalidProductNameGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenProductNameIsInvalid(AddPurchaseRequest request)
  {
    string validReceiptId = await GetValidReceiptId().ConfigureAwait(false);

    HttpResponseMessage response = await Send(validReceiptId, request).ConfigureAwait(false);
    string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    responseContent.Should().Contain("ProductName").And.Contain("EMPTY_PRODUCT_NAME");
  }

  private static async Task<HttpResponseMessage> Send(string receiptId, AddPurchaseRequest request)
  {
    using WebApplicationFactory<Program> webAppFactory = new TestWebApplicationFactory();
    HttpClient client = webAppFactory.CreateClient();
    return await client.PostAsJsonAsync($"/api/receipts/{receiptId}", request).ConfigureAwait(false);
  }

  private static async Task<string> GetValidReceiptId()
  {
    using WebApplicationFactory<Program> webAppFactory = new TestWebApplicationFactory();
    HttpClient client = webAppFactory.CreateClient();
    OpenNewReceiptRequest request = new("Store", todayDateOnly);
    HttpResponseMessage response = await client.PostAsJsonAsync("/api/receipts", request).ConfigureAwait(false);
    OpenNewReceiptResponse receipt
      = (await response.Content.ReadFromJsonAsync<OpenNewReceiptResponse>().ConfigureAwait(false))!;

    return receipt.Id;
  }
}
