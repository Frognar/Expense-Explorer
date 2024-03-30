namespace ExpenseExplorer.API.Tests.Receipts;

using System.Net;
using System.Net.Http.Json;
using ExpenseExplorer.API.Contract;
using Microsoft.AspNetCore.Mvc.Testing;

public class OpenNewReceiptTests
{
  public static IEnumerable<object[]> InvalidOpenNewRequestData
  {
    get
    {
      return new List<object[]>
      {
        new object[] { new OpenNewReceiptRequest(string.Empty, TodayDateOnly) },
        new object[] { new OpenNewReceiptRequest("store", TodayDateOnly.AddDays(1)) },
      };
    }
  }

  [Fact]
  public async Task CanAddReceipt()
  {
    OpenNewReceiptRequest request = new("store", TodayDateOnly);

    HttpResponseMessage response = await Send(request);

    response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
  }

  [Theory]
  [MemberData(nameof(InvalidOpenNewRequestData))]
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
