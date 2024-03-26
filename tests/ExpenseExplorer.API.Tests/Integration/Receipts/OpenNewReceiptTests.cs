namespace ExpenseExplorer.API.Tests.Integration.Receipts;

using System.Net;
using System.Net.Http.Json;

public class OpenNewReceiptTests(ReceiptApiFactory factory) : BaseIntegrationTest(factory)
{
  [Fact]
  public async Task CanCreateReceipt()
  {
    var result = await Client.PostAsJsonAsync(
      "api/receipts",
      new { storeName = "store", purchaseDate = DateOnly.MinValue });

    result.StatusCode.Should().Be(HttpStatusCode.OK);
  }
}
