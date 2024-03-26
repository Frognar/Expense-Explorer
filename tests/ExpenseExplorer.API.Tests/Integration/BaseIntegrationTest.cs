namespace ExpenseExplorer.API.Tests.Integration;

public abstract class BaseIntegrationTest(ReceiptApiFactory factory) : IClassFixture<ReceiptApiFactory>
{
  protected HttpClient Client { get; } = factory.CreateClient();
}
