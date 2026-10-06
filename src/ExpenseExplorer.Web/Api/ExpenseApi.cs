using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ExpenseExplorer.Contracts.Budget;
using ExpenseExplorer.Contracts.Dictionaries;
using ExpenseExplorer.Contracts.Logs;
using ExpenseExplorer.Contracts.ReceiptItems;
using ExpenseExplorer.Contracts.Receipts;
using ExpenseExplorer.Contracts.Reports;

namespace ExpenseExplorer.Web.Api;

/// <summary>Typed access to the API. Every call returns an <see cref="ApiResult{T}"/>; nothing throws for HTTP errors.</summary>
public sealed class ExpenseApi(HttpClient http)
{
    private const string Receipts = "api/v1/receipts";
    private const string Budget = "api/v1/budget";

    public Task<ApiResult<ReceiptListResponse>> ReceiptsAsync(ReceiptListRequest request) =>
        GetAsync<ReceiptListResponse>(Receipts + ListQueries.ToQuery(request));

    public Task<ApiResult<ReceiptResponse>> ReceiptAsync(Guid id) =>
        GetAsync<ReceiptResponse>($"{Receipts}/{id}");

    public Task<ApiResult<ReceiptResponse>> CreateReceiptAsync(CreateReceiptRequest request) =>
        SendAsync<ReceiptResponse>(HttpMethod.Post, Receipts, request);

    public Task<ApiResult<ReceiptResponse>> ChangeReceiptAsync(Guid id, UpdateReceiptRequest request) =>
        SendAsync<ReceiptResponse>(HttpMethod.Patch, $"{Receipts}/{id}", request);

    public Task<ApiResult<bool>> DeleteReceiptAsync(Guid id) =>
        SendAsync<bool>(HttpMethod.Delete, $"{Receipts}/{id}", null);

    public Task<ApiResult<ReceiptResponse>> DuplicateReceiptAsync(Guid id, DuplicateReceiptRequest request) =>
        SendAsync<ReceiptResponse>(HttpMethod.Post, $"{Receipts}/{id}/duplicate", request);

    public Task<ApiResult<ReceiptItemResponse>> AddItemAsync(Guid receiptId, ReceiptItemRequest request) =>
        SendAsync<ReceiptItemResponse>(HttpMethod.Post, $"{Receipts}/{receiptId}/items", request);

    public Task<ApiResult<ReceiptItemResponse>> ChangeItemAsync(Guid receiptId, Guid itemId, ReceiptItemRequest request) =>
        SendAsync<ReceiptItemResponse>(HttpMethod.Put, $"{Receipts}/{receiptId}/items/{itemId}", request);

    public Task<ApiResult<bool>> RemoveItemAsync(Guid receiptId, Guid itemId) =>
        SendAsync<bool>(HttpMethod.Delete, $"{Receipts}/{receiptId}/items/{itemId}", null);

    public async Task<ApiResult<DownloadedFile>> ExportCsvAsync(Guid receiptId)
    {
        using HttpResponseMessage response = await http.GetAsync(new Uri($"{Receipts}/{receiptId}/export.csv", UriKind.Relative));
        if (!response.IsSuccessStatusCode)
        {
            return ApiResult.Failure<DownloadedFile>(await ProblemAsync(response));
        }

        return ApiResult.Success(new DownloadedFile(
            response.Content.Headers.ContentDisposition?.FileNameStar ?? response.Content.Headers.ContentDisposition?.FileName ?? "receipt.csv",
            response.Content.Headers.ContentType?.ToString() ?? "text/csv",
            new ReadOnlyMemory<byte>(await response.Content.ReadAsByteArrayAsync())));
    }

    public async Task<ApiResult<ReceiptResponse>> ImportBiedronkaAsync(Stream file, string fileName)
    {
        using MultipartFormDataContent form = new();
        using StreamContent content = new(file);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        form.Add(content, "file", fileName);
        using HttpResponseMessage response = await http.PostAsync(new Uri($"{Receipts}/import/biedronka", UriKind.Relative), form);
        return await ReadAsync<ReceiptResponse>(response);
    }

    public async Task<ApiResult<PhotoImportResponse>> ImportPhotoAsync(Stream photo, string fileName, string contentType)
    {
        using MultipartFormDataContent form = new();
        using StreamContent content = new(photo);
        content.Headers.ContentType = MediaTypeHeaderValue.TryParse(contentType, out MediaTypeHeaderValue? type)
            ? type
            : new MediaTypeHeaderValue("image/jpeg");
        form.Add(content, "file", fileName);
        using HttpResponseMessage response = await http.PostAsync(new Uri($"{Receipts}/import/photo", UriKind.Relative), form);
        return await ReadAsync<PhotoImportResponse>(response);
    }

    public Task<ApiResult<ReceiptItemListResponse>> ReceiptItemsAsync(ReceiptItemListRequest request) =>
        GetAsync<ReceiptItemListResponse>("api/v1/receipt-items" + ListQueries.ToQuery(request));

    public Task<ApiResult<CategoryReportResponse>> CategoryReportAsync(DateOnly? from, DateOnly? to) =>
        GetAsync<CategoryReportResponse>("api/v1/reports/categories" + new QueryString().Add("from", from).Add("to", to));

    public Task<ApiResult<MonthlyReportResponse>> MonthlyReportAsync(int months) =>
        GetAsync<MonthlyReportResponse>("api/v1/reports/monthly" + new QueryString().Add("months", months));

    public Task<ApiResult<LogListResponse>> LogsAsync(LogListRequest request) =>
        GetAsync<LogListResponse>("api/v1/logs" + ListQueries.ToQuery(request));

    /// <summary>Known stores, item names or categories containing <paramref name="search"/>, for suggestions.</summary>
    public Task<ApiResult<IReadOnlyList<string>>> SuggestionsAsync(Suggestions kind, string? search) =>
        GetAsync<IReadOnlyList<string>>(
            $"api/v1/{PathOf(kind)}"
            + new QueryString().Add("search", search).Add("limit", 50));

    /// <summary>Names of one kind in use, with how often and how recently, for tidying them up.</summary>
    public Task<ApiResult<IReadOnlyList<NameUsageResponse>>> NameUsageAsync(Suggestions kind, string? search, int limit) =>
        GetAsync<IReadOnlyList<NameUsageResponse>>(
            $"api/v1/dictionaries/{PathOf(kind)}" + new QueryString().Add("search", search).Add("limit", limit));

    public Task<ApiResult<RenameNameResponse>> RenameAsync(Suggestions kind, RenameNameRequest request) =>
        SendAsync<RenameNameResponse>(HttpMethod.Post, $"api/v1/dictionaries/{PathOf(kind)}/rename", request);

    public Task<ApiResult<IReadOnlyList<PeriodResponse>>> BudgetPeriodsAsync() =>
        GetAsync<IReadOnlyList<PeriodResponse>>($"{Budget}/periods");

    /// <summary>The last periods that have started, oldest first.</summary>
    public Task<ApiResult<IReadOnlyList<PeriodResultResponse>>> BudgetHistoryAsync(int count) =>
        GetAsync<IReadOnlyList<PeriodResultResponse>>($"{Budget}/history" + new QueryString().Add("count", count));

    /// <summary>The period covering today, or the given one.</summary>
    public Task<ApiResult<BudgetResponse>> BudgetAsync(Guid? periodId) =>
        GetAsync<BudgetResponse>($"{Budget}/periods/{(periodId is { } id ? id.ToString() : "current")}");

    public Task<ApiResult<BudgetResponse>> CreatePeriodAsync(CreatePeriodRequest request) =>
        SendAsync<BudgetResponse>(HttpMethod.Post, $"{Budget}/periods", request);

    public Task<ApiResult<bool>> SaveFundAsync(Guid periodId, Guid? fundId, FundRequest request) =>
        fundId is { } id
            ? SendAsync<bool>(HttpMethod.Put, $"{Budget}/periods/{periodId}/funds/{id}", request)
            : SendAsync<bool>(HttpMethod.Post, $"{Budget}/periods/{periodId}/funds", request);

    public Task<ApiResult<bool>> RemoveFundAsync(Guid periodId, Guid fundId) =>
        SendAsync<bool>(HttpMethod.Delete, $"{Budget}/periods/{periodId}/funds/{fundId}", null);

    public Task<ApiResult<bool>> SavePlanItemAsync(Guid periodId, Guid? itemId, PlanItemRequest request) =>
        itemId is { } id
            ? SendAsync<bool>(HttpMethod.Put, $"{Budget}/periods/{periodId}/items/{id}", request)
            : SendAsync<bool>(HttpMethod.Post, $"{Budget}/periods/{periodId}/items", request);

    public Task<ApiResult<bool>> RemovePlanItemAsync(Guid periodId, Guid itemId) =>
        SendAsync<bool>(HttpMethod.Delete, $"{Budget}/periods/{periodId}/items/{itemId}", null);

    public Task<ApiResult<IReadOnlyList<GroupResponse>>> BudgetGroupsAsync() =>
        GetAsync<IReadOnlyList<GroupResponse>>($"{Budget}/groups");

    public Task<ApiResult<bool>> SaveBudgetGroupAsync(Guid? groupId, GroupRequest request) =>
        groupId is { } id
            ? SendAsync<bool>(HttpMethod.Put, $"{Budget}/groups/{id}", request)
            : SendAsync<bool>(HttpMethod.Post, $"{Budget}/groups", request);

    public Task<ApiResult<bool>> DeleteBudgetGroupAsync(Guid groupId) =>
        SendAsync<bool>(HttpMethod.Delete, $"{Budget}/groups/{groupId}", null);

    public Task<ApiResult<bool>> SetCategoryGroupAsync(CategoryGroupRequest request) =>
        SendAsync<bool>(HttpMethod.Put, $"{Budget}/categories", request);

    public Task<ApiResult<TemplateResponse>> BudgetTemplateAsync() =>
        GetAsync<TemplateResponse>($"{Budget}/template");

    public Task<ApiResult<bool>> SaveBudgetTemplateAsync(TemplateRequest request) =>
        SendAsync<bool>(HttpMethod.Put, $"{Budget}/template", request);

    public static string PathOf(Suggestions kind) => kind switch
    {
        Suggestions.Stores => "stores",
        Suggestions.Items => "items",
        _ => "categories",
    };

    private async Task<ApiResult<T>> GetAsync<T>(string url)
    {
        using HttpResponseMessage response = await http.GetAsync(new Uri(url, UriKind.Relative));
        return await ReadAsync<T>(response);
    }

    private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string url, object? body)
    {
        using HttpRequestMessage request = new(method, new Uri(url, UriKind.Relative));
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        using HttpResponseMessage response = await http.SendAsync(request);
        return await ReadAsync<T>(response);
    }

    private static async Task<ApiResult<T>> ReadAsync<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            return ApiResult.Failure<T>(await ProblemAsync(response));
        }

        // Endpoints answering 204 No Content are read as bool "done".
        if (response.StatusCode == HttpStatusCode.NoContent || typeof(T) == typeof(bool))
        {
            return ApiResult.Success((T)(object)true);
        }

        T? value = await response.Content.ReadFromJsonAsync<T>();
        return value is null
            ? ApiResult.Failure<T>(new ApiProblem(response.StatusCode, new Dictionary<string, string[]> { [""] = ["Http.EmptyResponse"] }))
            : ApiResult.Success(value);
    }

    /// <summary>Error codes from a ProblemDetails body, or one code naming the HTTP status when there is none.</summary>
    private static async Task<ApiProblem> ProblemAsync(HttpResponseMessage response)
    {
        Dictionary<string, string[]>? codes = null;
        try
        {
            using JsonDocument problem = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            if (problem.RootElement.ValueKind == JsonValueKind.Object
                && problem.RootElement.TryGetProperty("errorCodes", out JsonElement errorCodes))
            {
                codes = errorCodes.Deserialize<Dictionary<string, string[]>>();
            }
        }
        catch (JsonException)
        {
            // Not a ProblemDetails body; fall back to the status code below.
        }

        return new ApiProblem(
            response.StatusCode,
            codes is { Count: > 0 } ? codes : new Dictionary<string, string[]> { [""] = [$"Http.{(int)response.StatusCode}"] });
    }
}

public enum Suggestions
{
    Stores,
    Items,
    Categories,
}

public sealed record DownloadedFile(string Name, string ContentType, ReadOnlyMemory<byte> Content);
