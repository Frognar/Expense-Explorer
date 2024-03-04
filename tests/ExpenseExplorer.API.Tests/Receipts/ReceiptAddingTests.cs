using System.Net;
using System.Net.Http.Json;
using ExpenseExplorer.API.Contract;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ExpenseExplorer.API.Tests.Receipts;

public class ReceiptAddingTests {
  [Property(Arbitrary = [typeof(NonEmptyStringGenerator), typeof(DateOnlyGenerator)])]
  public void ContainsDataGivenDuringConstruction(string storeName, DateOnly purchaseDate) {
    OpenNewReceiptRequest request = new(storeName, purchaseDate);
    request.StoreName.Should().Be(storeName);
    request.PurchaseDate.Should().Be(purchaseDate);
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator), typeof(NonFutureDateOnlyGenerator)], MaxTest = 10)]
  public async Task CanAddReceipt(string storeName, DateOnly purchaseDate) {
    OpenNewReceiptRequest request = new(storeName, purchaseDate);

    HttpResponseMessage response = await Send(request);

    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator), typeof(FutureDateOnlyGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenReceiptInFuture(string storeName, DateOnly purchaseDate) {
    OpenNewReceiptRequest request = new(storeName, purchaseDate);

    HttpResponseMessage response = await Send(request);
    string responseContent = await response.Content.ReadAsStringAsync();

    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    responseContent.Should().Contain("PurchaseDate").And.Contain("FUTURE_DATE");
  }

  [Property(Arbitrary = [typeof(EmptyStringGenerator), typeof(NonFutureDateOnlyGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenStoreNameIsEmpty(string storeName, DateOnly purchaseDate) {
    OpenNewReceiptRequest request = new(storeName, purchaseDate);

    HttpResponseMessage response = await Send(request);
    string responseContent = await response.Content.ReadAsStringAsync();

    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    responseContent.Should().Contain("StoreName").And.Contain("EMPTY_STORE_NAME");
  }

  private static Task<HttpResponseMessage> Send(OpenNewReceiptRequest request) {
    WebApplicationFactory<Program> webAppFactory = new TestWebApplicationFactory();
    HttpClient client = webAppFactory.CreateClient();
    return client.PostAsJsonAsync("/api/receipts", request);
  }
}
