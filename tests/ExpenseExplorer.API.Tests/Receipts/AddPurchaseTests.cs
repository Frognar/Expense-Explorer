namespace ExpenseExplorer.API.Tests.Receipts;

using System.Net;
using System.Net.Http.Json;
using ExpenseExplorer.API.Contract;
using Microsoft.AspNetCore.Mvc.Testing;

public class AddPurchaseTests
{
  public static IEnumerable<object[]> ValidAddPurchaseRequestData
  {
    get
    {
      return new List<object[]>
      {
        new object[] { new AddPurchaseRequest("item", "category", 1, 0, 0, null), },
        new object[] { new AddPurchaseRequest("item", "category", 1, 1, 1, null) },
        new object[] { new AddPurchaseRequest("item", "category", 1, 1, null, "description") },
      };
    }
  }

  public static IEnumerable<object[]> InvalidAddPurchaseRequestData
  {
    get
    {
      return new List<object[]>
      {
        new object[] { new AddPurchaseRequest(string.Empty, "category", 1, 0, null, null) },
        new object[] { new AddPurchaseRequest("item", string.Empty, 1, 0, null, null) },
        new object[] { new AddPurchaseRequest("item", "category", 0, 0, null, null) },
        new object[] { new AddPurchaseRequest("item", "category", 1, -1, null, null) },
        new object[] { new AddPurchaseRequest("item", "category", 1, 0, -1, null) },
      };
    }
  }

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

  [Theory]
  [MemberData(nameof(ValidAddPurchaseRequestData))]
  public async Task CanAddPurchaseToReceipt(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request);
    response.StatusCode.ShouldBeIn200Group();
  }

  [Theory]
  [MemberData(nameof(ValidAddPurchaseRequestData))]
  public async Task ContainsAddedPurchaseInResponse(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await SendWithValidReceiptId(request);
    OpenNewReceiptResponse receipt = (await response.Content.ReadFromJsonAsync<OpenNewReceiptResponse>())!;
    receipt.Purchases.Count().Should().Be(1);
  }

  [Theory]
  [MemberData(nameof(ValidAddPurchaseRequestData))]
  public async Task IsNotFoundWhenReceiptIdIsInvalid(AddPurchaseRequest request)
  {
    HttpResponseMessage response = await Send("/api/receipts/invalid-id", request);
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Theory]
  [MemberData(nameof(InvalidAddPurchaseRequestData))]
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
