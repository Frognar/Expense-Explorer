namespace ExpenseExplorer.API.Tests.Receipts;

using System.Net;
using System.Net.Http.Json;
using ExpenseExplorer.API.Contract;
using Microsoft.AspNetCore.Mvc.Testing;

public class OpenNewReceiptTests
{
  [Property(Arbitrary = [typeof(ValidOpenNewReceiptRequestGenerator)], MaxTest = 25)]
  public async Task CanAddReceipt(OpenNewReceiptRequest request)
  {
    HttpResponseMessage response = await Send(request);

    response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
  }

  [Property(Arbitrary = [typeof(InvalidOpenNewReceiptRequestGenerator)], MaxTest = 25)]
  public async Task IsBadRequestWhenInvalidRequest(OpenNewReceiptRequest request)
  {
    HttpResponseMessage response = await Send(request);

    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  private static async Task<HttpResponseMessage> Send(OpenNewReceiptRequest request)
  {
    using WebApplicationFactory<Program> webAppFactory = new TestWebApplicationFactory();
    HttpClient client = webAppFactory.CreateClient();
    return await client.PostAsJsonAsync("/api/receipts", request);
  }
}
