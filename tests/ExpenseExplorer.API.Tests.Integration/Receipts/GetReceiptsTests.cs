namespace ExpenseExplorer.API.Tests.Integration.Receipts;

public class GetReceiptsTests(ReceiptApiFactory factory) : BaseIntegrationTest(factory), IAsyncLifetime
{
  private const int _totalReceipts = 51;
  private readonly DateOnly _today = new(2000, 1, 1);

  public async Task InitializeAsync()
  {
    IServiceScope scope = ServiceScopeFactory.CreateScope();
    ExpenseExplorerContext dbContext = scope.ServiceProvider.GetRequiredService<ExpenseExplorerContext>();
    if (await dbContext.Receipts.AnyAsync())
    {
      return;
    }

    DbReceipt CreateReceiptHeader(int i)
      => new(Guid.NewGuid().ToString("N"), $"store_{i}", _today.AddDays(-i % 5), (i % 10) + .5m);

    IEnumerable<DbReceipt> receiptHeaders = Enumerable.Range(1, _totalReceipts).Select(CreateReceiptHeader);
    await dbContext.Receipts.AddRangeAsync(receiptHeaders);
    await dbContext.SaveChangesAsync();
  }

  public Task DisposeAsync()
  {
    return Task.CompletedTask;
  }

  [Fact]
  public async Task CanGetReceipts()
  {
    HttpResponseMessage result = await Get();
    result.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task ReturnsTotalCountInResponse()
  {
    GetReceiptsResponse response = await GetReceipts();
    response.TotalCount.Should().Be(_totalReceipts);
  }

  [Property]
  public async Task ReturnsPageSizeInResponse(int pageSize)
  {
    int expectedPageSize = pageSize < 1
      ? GetReceiptsQuery.DefaultPageSize
      : Math.Min(pageSize, GetReceiptsQuery.MaxPageSize);

    GetReceiptsResponse response = await GetReceipts($"?pageSize={pageSize}");
    response.PageSize.Should().Be(expectedPageSize);
  }

  [Property]
  public async Task ReturnsPageCountInResponse(int pageSize)
  {
    pageSize = pageSize < 1
      ? GetReceiptsQuery.DefaultPageSize
      : Math.Min(pageSize, GetReceiptsQuery.MaxPageSize);

    GetReceiptsResponse response = await GetReceipts($"?pageSize={pageSize}");
    int expectedPageCount = (int)Math.Ceiling((double)_totalReceipts / pageSize);
    response.PageCount.Should().Be(expectedPageCount);
  }

  [Theory]
  [InlineData(1)]
  [InlineData(2)]
  public async Task ReturnsPageNumberInResponse(int pageNumber)
  {
    GetReceiptsResponse response = await GetReceipts($"?pageNumber={pageNumber}");
    response.PageNumber.Should().Be(pageNumber);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public async Task ReturnsFirstPageWhenInvalidPageNumber(int pageNumber)
  {
    GetReceiptsResponse response = await GetReceipts($"?pageNumber={pageNumber}");
    response.PageNumber.Should().Be(1);
  }

  [Fact]
  public async Task ReturnsPageOfReceipts()
  {
    GetReceiptsResponse response = await GetReceipts();
    response.Receipts.Should().HaveCount(GetReceiptsQuery.DefaultPageSize);
  }

  [Fact]
  public async Task OrdersReceiptsByPurchaseDateByDefault()
  {
    GetReceiptsResponse response = await GetReceipts("?pageSize=50");
    response.Receipts.Should()
      .BeInAscendingOrder(r => r.PurchaseDate)
      .And.ThenBeInDescendingOrder(r => r.Id);
  }

  [Fact]
  public async Task CanRetrieveNextPage()
  {
    GetReceiptsResponse firstPage = await GetReceipts();
    GetReceiptsResponse secondPage = await GetReceipts("?pageNumber=2");
    firstPage.Receipts.Should()
      .NotContain(r => secondPage.Receipts.Any(r2 => r.Id == r2.Id));
  }

  [Theory]
  [InlineData(1)]
  [InlineData(5)]
  [InlineData(25)]
  public async Task CanDefinePageSize(int pageSize)
  {
    GetReceiptsResponse response = await GetReceipts($"?pageSize={pageSize}");
    response.Receipts.Should().HaveCount(pageSize);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public async Task ReturnsDefaultPageSizeWhenInvalid(int pageSize)
  {
    GetReceiptsResponse response = await GetReceipts($"?pageSize={pageSize}");
    response.Receipts.Should().HaveCount(GetReceiptsQuery.DefaultPageSize);
  }

  [Fact]
  public async Task ReturnsMaxPageSize()
  {
    GetReceiptsResponse response = await GetReceipts($"?pageSize={GetReceiptsQuery.MaxPageSize + 1}");
    response.Receipts.Should().HaveCount(GetReceiptsQuery.MaxPageSize);
  }

  [Fact]
  public async Task ReturnsFilteredReceipts()
  {
    string search = "E_1";
    DateOnly after = _today.AddDays(-5);
    DateOnly before = _today.AddDays(-1);
    decimal minTotal = 1.0m;
    decimal maxTotal = 5.0m;
    string parameters = $"?search={search}&after={after}&before={before}&minTotal={minTotal}&maxTotal={maxTotal}";

    GetReceiptsResponse response = await GetReceipts(parameters);

    response.Receipts.Should()
      .OnlyContain(
        r =>
          r.Store.Contains(search, StringComparison.OrdinalIgnoreCase)
          && r.PurchaseDate >= after
          && r.PurchaseDate <= before
          && r.Total >= minTotal
          && r.Total <= maxTotal);
  }

  private async Task<GetReceiptsResponse> GetReceipts(string parameters = "")
  {
    HttpResponseMessage result = await Get(parameters);
    return (await result.Content.ReadFromJsonAsync<GetReceiptsResponse>())!;
  }

  private async Task<HttpResponseMessage> Get(string parameters = "")
  {
    Uri uri = new("api/receipts" + parameters, UriKind.Relative);
    return await Client.GetAsync(uri);
  }
}
