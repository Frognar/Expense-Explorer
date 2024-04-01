namespace ExpenseExplorer.API.Tests.Integration.Receipts;

public class GetReceiptsTests(ReceiptApiFactory factory) : BaseIntegrationTest(factory)
{
  [Fact]
  public async Task CanGetReceipts()
  {
    Uri uri = new("api/receipts", UriKind.Relative);
    var result = await Client.GetAsync(uri);
    result.StatusCode.ShouldBeIn200Group();
  }
}
