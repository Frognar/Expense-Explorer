using System.Net;
using System.Net.Http.Json;
using ExpenseExplorer.API.Contract;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ExpenseExplorer.API.Tests;

public class ReceiptAddingTests {
  [Property(Arbitrary = [typeof(NonEmptyStringGenerator), typeof(DateOnlyGenerator)])]
  public void ContainsDataGivenDuringConstruction(string storeName, DateOnly purchaseDate) {
    OpenNewReceiptRequest request = new(storeName, purchaseDate);
    request.StoreName.Should().Be(storeName);
    request.PurchaseDate.Should().Be(purchaseDate);
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator), typeof(NonFutureDateOnlyGenerator)], MaxTest = 10)]
  public async Task CanAddReceipt(string storeName, DateOnly purchaseDate) {
    WebApplicationFactory<Program> webAppFactory = new();
    HttpClient client = webAppFactory.CreateClient();
    OpenNewReceiptRequest request = new(storeName, purchaseDate);

    HttpResponseMessage response = await client.PostAsJsonAsync("/api/receipts", request);
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator), typeof(FutureDateOnlyGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenReceiptInFuture(string storeName, DateOnly purchaseDate) {
    WebApplicationFactory<Program> webAppFactory = new();
    HttpClient client = webAppFactory.CreateClient();
    OpenNewReceiptRequest request = new(storeName, purchaseDate);

    HttpResponseMessage response = await client.PostAsJsonAsync("/api/receipts", request);
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }
}
