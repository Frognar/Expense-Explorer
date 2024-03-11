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
  public async Task CanAddPurchaseToReceipt(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request);
    response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
  }

  [Property(Arbitrary = [typeof(AddPurchaseRequestWithInvalidProductNameGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenProductNameIsInvalid(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request);
    await AssertBadRequest(response, ["ProductName", "EMPTY_PRODUCT_NAME"]);
  }

  [Property(Arbitrary = [typeof(AddPurchaseRequestWithInvalidProductCategoryGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenProductCategoryIsInvalid(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request);
    await AssertBadRequest(response, ["ProductCategory", "EMPTY_PRODUCT_CATEGORY"]);
  }

  [Property(Arbitrary = [typeof(AddPurchaseRequestWithInvalidQuantityGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenQuantityIsInvalid(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request);
    await AssertBadRequest(response, ["Quantity", "NON_POSITIVE_QUANTITY"]);
  }

  [Property(Arbitrary = [typeof(AddPurchaseRequestWithInvalidUnitPriceGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenUnitPriceIsInvalid(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request);
    await AssertBadRequest(response, ["UnitPrice", "NEGATIVE_UNIT_PRICE"]);
  }

  [Property(Arbitrary = [typeof(AddPurchaseRequestWithInvalidTotalDiscountGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenTotalDiscountIsInvalid(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request);
    await AssertBadRequest(response, ["TotalDiscount", "NEGATIVE_TOTAL_DISCOUNT"]);
  }

  [Property(Arbitrary = [typeof(AddPurchaseRequestWithInvalidDescriptionGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenTotalDescriptionIsInvalid(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request);
    await AssertBadRequest(response, ["Description", "EMPTY_DESCRIPTION"]);
  }

  [Property(Arbitrary = [typeof(ValidAddPurchaseRequestGenerator)], MaxTest = 10)]
  public async Task IsNotFoundWhenReceiptIdIsInvalid(AddPurchaseRequest request)
  {
    using WebApplicationFactory<Program> webAppFactory = new TestWebApplicationFactory();
    HttpClient client = webAppFactory.CreateClient();
    HttpResponseMessage response
      = await client.PostAsJsonAsync("/api/receipts/invalid-id", request);

    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  private static async Task<HttpResponseMessage> SendWithValidReceiptId(AddPurchaseRequest request)
  {
    using WebApplicationFactory<Program> webAppFactory = new TestWebApplicationFactory();
    HttpClient client = webAppFactory.CreateClient();
    OpenNewReceiptRequest openReceiptRequest = new("Store", TodayDateOnly);
    HttpResponseMessage response
      = await client.PostAsJsonAsync("/api/receipts", openReceiptRequest);

    OpenNewReceiptResponse receipt
      = (await response.Content.ReadFromJsonAsync<OpenNewReceiptResponse>())!;

    return await client.PostAsJsonAsync($"/api/receipts/{receipt.Id}", request);
  }

  private static async Task AssertBadRequest(HttpResponseMessage response, List<string> errorsToCheck)
  {
    string responseContent = await response.Content.ReadAsStringAsync();
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    errorsToCheck.ForEach(e => responseContent.Should().Contain(e));
  }
}
