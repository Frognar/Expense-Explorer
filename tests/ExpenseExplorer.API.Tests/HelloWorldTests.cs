using Microsoft.AspNetCore.Mvc.Testing;

namespace ExpenseExplorer.API.Tests;

public class HelloWorldTests {
  [Fact]
  public async Task HelloWorldTest() {
    WebApplicationFactory<Program> webAppFactory = new();
    HttpClient client = webAppFactory.CreateClient();

    HttpResponseMessage response = await client.GetAsync("/hello-world");
    string responseString = await response.Content.ReadAsStringAsync();

    Assert.Equal("Hello World!", responseString);
  }
}
