namespace ExpenseExplorer.API.Tests.Integration.Receipts;

using ExpenseExplorer.API.Tests.Integration.Receipts.TestData;

public class OpenNewReceiptTests(ReceiptApiFactory factory) : BaseIntegrationTest(factory)
{
  [Theory]
  [ClassData(typeof(ValidOpenNewRequestData))]
  public async Task CanCreateReceipt(object request)
  {
    HttpResponseMessage response = await Post(request);
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    response.Headers.Location.Should().NotBeNull();
  }

  [Theory]
  [ClassData(typeof(InvalidOpenNewRequestData))]
  public async Task IsBadRequestWhenInvalidRequest(object request)
  {
    HttpResponseMessage response = await Post(request);
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  private async Task<HttpResponseMessage> Post(object request)
  {
    return await Client.PostAsJsonAsync("api/receipts", request);
  }
}
