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
    HttpResponseMessage response = await SendWithValidReceiptId(request).ConfigureAwait(false);
    response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
  }

  [Property(Arbitrary = [typeof(AddPurchaseRequestWithInvalidProductNameGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenProductNameIsInvalid(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request).ConfigureAwait(false);
    await AssertBadRequest(response, ["ProductName", "EMPTY_PRODUCT_NAME"]).ConfigureAwait(false);
  }

  [Property(Arbitrary = [typeof(AddPurchaseRequestWithInvalidProductCategoryGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenProductCategoryIsInvalid(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request).ConfigureAwait(false);
    await AssertBadRequest(response, ["ProductCategory", "EMPTY_PRODUCT_CATEGORY"]).ConfigureAwait(false);
  }

  [Property(Arbitrary = [typeof(AddPurchaseRequestWithInvalidQuantityGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenQuantityIsInvalid(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request).ConfigureAwait(false);
    await AssertBadRequest(response, ["Quantity", "NON_POSITIVE_QUANTITY"]).ConfigureAwait(false);
  }

  [Property(Arbitrary = [typeof(AddPurchaseRequestWithInvalidUnitPriceGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenUnitPriceIsInvalid(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request).ConfigureAwait(false);
    await AssertBadRequest(response, ["UnitPrice", "NEGATIVE_UNIT_PRICE"]).ConfigureAwait(false);
  }

  [Property(Arbitrary = [typeof(AddPurchaseRequestWithInvalidTotalDiscountGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenTotalDiscountIsInvalid(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request).ConfigureAwait(false);
    await AssertBadRequest(response, ["TotalDiscount", "NEGATIVE_TOTAL_DISCOUNT"]).ConfigureAwait(false);
  }

  [Property(Arbitrary = [typeof(AddPurchaseRequestWithInvalidDescriptionGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenTotalDescriptionIsInvalid(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request).ConfigureAwait(false);
    await AssertBadRequest(response, ["Description", "EMPTY_DESCRIPTION"]).ConfigureAwait(false);
  }

  [Property(Arbitrary = [typeof(ValidAddPurchaseRequestGenerator)])]
  public async Task IsNotFoundWhenReceiptIdIsInvalid(AddPurchaseRequest request)
  {
    using WebApplicationFactory<Program> webAppFactory = new TestWebApplicationFactory();
    HttpClient client = webAppFactory.CreateClient();
    HttpResponseMessage response
      = await client.PostAsJsonAsync("/api/receipts/invalid-id", request).ConfigureAwait(false);

    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  private static async Task<HttpResponseMessage> SendWithValidReceiptId(AddPurchaseRequest request)
  {
    using WebApplicationFactory<Program> webAppFactory = new TestWebApplicationFactory();
    HttpClient client = webAppFactory.CreateClient();
    OpenNewReceiptRequest openReceiptRequest = new("Store", TodayDateOnly);
    HttpResponseMessage response
      = await client.PostAsJsonAsync("/api/receipts", openReceiptRequest).ConfigureAwait(false);

    OpenNewReceiptResponse receipt
      = (await response.Content.ReadFromJsonAsync<OpenNewReceiptResponse>().ConfigureAwait(false))!;

    return await client.PostAsJsonAsync($"/api/receipts/{receipt.Id}", request).ConfigureAwait(false);
  }

  private static async Task AssertBadRequest(HttpResponseMessage response, List<string> errorsToCheck)
  {
    string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    errorsToCheck.ForEach(e => responseContent.Should().Contain(e));
  }
}
