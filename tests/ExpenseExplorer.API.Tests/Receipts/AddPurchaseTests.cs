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

  [Property(
    Arbitrary = [typeof(NonEmptyStringGenerator), typeof(NonFutureDateOnlyGenerator), typeof(PositiveDecimalGenerator)],
    MaxTest = 10)]
  public async Task CanAddReceipt(
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

    HttpResponseMessage response = await Send("1234", request).ConfigureAwait(false);

    response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
  }

  private static async Task<HttpResponseMessage> Send(string receiptId, AddPurchaseRequest request)
  {
    using WebApplicationFactory<Program> webAppFactory = new TestWebApplicationFactory();
    HttpClient client = webAppFactory.CreateClient();
    return await client.PostAsJsonAsync($"/api/receipts/{receiptId}", request).ConfigureAwait(false);
  }
}
