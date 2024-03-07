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

    HttpResponseMessage response = await Send(request).ConfigureAwait(false);

    response.EnsureSuccessStatusCode();
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator), typeof(FutureDateOnlyGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenReceiptInFuture(string storeName, DateOnly purchaseDate) {
    OpenNewReceiptRequest request = new(storeName, purchaseDate);

    HttpResponseMessage response = await Send(request).ConfigureAwait(false);
    string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    responseContent.Should().Contain("PurchaseDate").And.Contain("FUTURE_DATE");
  }

  [Property(Arbitrary = [typeof(EmptyStringGenerator), typeof(NonFutureDateOnlyGenerator)], MaxTest = 10)]
  public async Task IsBadRequestWhenStoreNameIsEmpty(string storeName, DateOnly purchaseDate) {
    OpenNewReceiptRequest request = new(storeName, purchaseDate);

    HttpResponseMessage response = await Send(request).ConfigureAwait(false);
    string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    responseContent.Should().Contain("StoreName").And.Contain("EMPTY_STORE_NAME");
  }

  [Property(Arbitrary = [typeof(EmptyStringGenerator), typeof(FutureDateOnlyGenerator)], MaxTest = 10)]
  public async Task AllValidationErrorsAreCollectedAndReturnedTogether(string storeName, DateOnly purchaseDate) {
    OpenNewReceiptRequest request = new(storeName, purchaseDate);

    HttpResponseMessage response = await Send(request).ConfigureAwait(false);
    string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    responseContent.Should()
      .Contain("StoreName")
      .And.Contain("EMPTY_STORE_NAME")
      .And.Contain("PurchaseDate")
      .And.Contain("FUTURE_DATE");
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator), typeof(NonFutureDateOnlyGenerator)], MaxTest = 10)]
  public async Task ReturnsCreatedReceiptWhenValid(string storeName, DateOnly purchaseDate) {
    OpenNewReceiptRequest request = new(storeName, purchaseDate);

    HttpResponseMessage response = await Send(request).ConfigureAwait(false);
    OpenNewReceiptResponse receipt
      = (await response.Content.ReadFromJsonAsync<OpenNewReceiptResponse>().ConfigureAwait(false))!;

    receipt.Id.Should().NotBeEmpty();
    receipt.StoreName.Should().Be(storeName.Trim());
    receipt.PurchaseDate.Should().Be(purchaseDate);
  }

  private static async Task<HttpResponseMessage> Send(OpenNewReceiptRequest request) {
    using WebApplicationFactory<Program> webAppFactory = new TestWebApplicationFactory();
    HttpClient client = webAppFactory.CreateClient();
    return await client.PostAsJsonAsync("/api/receipts", request).ConfigureAwait(false);
  }
}
