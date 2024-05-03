namespace ExpenseExplorer.API.Tests.Integration;

public abstract class BaseIntegrationTest(ReceiptApiFactory factory) : IClassFixture<ReceiptApiFactory>
{
  protected HttpClient Client { get; } = factory.CreateClient();

  protected IServiceScopeFactory ServiceScopeFactory { get; }
    = factory.Services.GetRequiredService<IServiceScopeFactory>();
}
