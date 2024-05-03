namespace ExpenseExplorer.API.Tests.Integration.Receipts;

public class GetReceiptTests(ReceiptApiFactory factory) : BaseIntegrationTest(factory), IAsyncLifetime
{
  private static readonly DateOnly _today = new(2000, 1, 1);

  private static readonly Dictionary<string, DbReceipt> _receipts = new();

  public async Task InitializeAsync()
  {
    IServiceScope scope = ServiceScopeFactory.CreateScope();
    ExpenseExplorerContext dbContext = scope.ServiceProvider.GetRequiredService<ExpenseExplorerContext>();
    if (await dbContext.Receipts.AnyAsync())
    {
      return;
    }

    DbReceipt receipt1 = new("abc", "store", _today.AddDays(-1), 0);
    DbPurchase purchase1 = new(receipt1.Id, "a", "item", "category", 1, 5, 0, string.Empty);
    receipt1.Purchases.Add(purchase1);
    receipt1.Total = receipt1.Purchases.Sum(p => (p.UnitPrice * p.Quantity) - p.TotalDiscount);
    _receipts[receipt1.Id] = receipt1;

    DbReceipt receipt2 = new("bcd", "store", _today.AddDays(-1), 0);
    DbPurchase purchase2 = new(receipt2.Id, "a", "item 1", "category", 2, 1.5m, 0.5m, "desc");
    DbPurchase purchase3 = new(receipt2.Id, "b", "item 2", "other category", 12, 0.5m, 0.5m, string.Empty);
    receipt2.Purchases.Add(purchase2);
    receipt2.Purchases.Add(purchase3);
    receipt2.Total = receipt2.Purchases.Sum(p => (p.UnitPrice * p.Quantity) - p.TotalDiscount);
    _receipts[receipt2.Id] = receipt2;

    await dbContext.AddRangeAsync(_receipts.Values);
    await dbContext.SaveChangesAsync();
  }

  public Task DisposeAsync()
  {
    return Task.CompletedTask;
  }

  [Theory]
  [InlineData("abc")]
  [InlineData("bcd")]
  public async Task CanGetReceipt(string id)
  {
    HttpResponseMessage response = await Get(id);
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task NotFoundForUnknownReceipt()
  {
    HttpResponseMessage response = await Get("unknown");
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Theory]
  [InlineData("abc")]
  [InlineData("bcd")]
  public async Task ReturnsReceipt(string id)
  {
    HttpResponseMessage response = await Get(id);
    GetReceiptResponse receipt = (await response.Content.ReadFromJsonAsync<GetReceiptResponse>())!;
    AssertReceipt(receipt, _receipts[id]);
  }

  [Theory]
  [InlineData("abc")]
  [InlineData("bcd")]
  public async Task ReturnsReceiptWithPurchases(string id)
  {
    HttpResponseMessage response = await Get(id);
    GetReceiptResponse receipt = (await response.Content.ReadFromJsonAsync<GetReceiptResponse>())!;
    AssertPurchases(receipt.Purchases, _receipts[id].Purchases);
  }

  private static void AssertReceipt(GetReceiptResponse receiptResponse, DbReceipt receipt)
  {
    receiptResponse.Should().NotBeNull();
    receiptResponse.Id.Should().Be(receipt.Id);
    receiptResponse.Store.Should().Be(receipt.Store);
    receiptResponse.PurchaseDate.Should().Be(receipt.PurchaseDate);
    receiptResponse.Total.Should().Be(receipt.Total);
    receiptResponse.Purchases.Count().Should().Be(receipt.Purchases.Count);
  }

  private static void AssertPurchases(
    IEnumerable<GetReceiptPurchaseResponse> purchaseResponses,
    IEnumerable<DbPurchase> purchases)
  {
    purchaseResponses.Zip(purchases, (response, purchase) => (response, purchase))
      .ToList()
      .ForEach(pair => AssertPurchase(pair.response, pair.purchase));
  }

  private static void AssertPurchase(GetReceiptPurchaseResponse purchaseResponse, DbPurchase purchase)
  {
    purchaseResponse.Should().NotBeNull();
    purchaseResponse.Id.Should().Be(purchase.PurchaseId);
    purchaseResponse.Item.Should().Be(purchase.Item);
    purchaseResponse.Category.Should().Be(purchase.Category);
    purchaseResponse.Quantity.Should().Be(purchase.Quantity);
    purchaseResponse.UnitPrice.Should().Be(purchase.UnitPrice);
    purchaseResponse.TotalDiscount.Should().Be(purchase.TotalDiscount);
    purchaseResponse.Description.Should().Be(purchase.Description);
  }

  private async Task<HttpResponseMessage> Get(string id)
  {
    Uri uri = new($"api/receipts/{id}", UriKind.Relative);
    return await Client.GetAsync(uri);
  }
}
