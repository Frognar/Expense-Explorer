namespace ExpenseExplorer.API.Tests.Integration.Receipts;

using System.Net;
using System.Net.Http.Json;
using ExpenseExplorer.API.Tests.Integration.Receipts.TestData;

public class OpenNewReceiptTests(ReceiptApiFactory factory) : BaseIntegrationTest(factory)
{
  [Theory]
  [ClassData(typeof(ValidOpenNewRequestData))]
  public async Task CanCreateReceipt(object request)
  {
    var response = await Client.PostAsJsonAsync("api/receipts", request);
    response.StatusCode.ShouldBeIn200Group();
  }

  [Theory]
  [ClassData(typeof(InvalidOpenNewRequestData))]
  public async Task IsBadRequestWhenInvalidRequest(object request)
  {
    var response = await Client.PostAsJsonAsync("api/receipts", request);
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }
}
