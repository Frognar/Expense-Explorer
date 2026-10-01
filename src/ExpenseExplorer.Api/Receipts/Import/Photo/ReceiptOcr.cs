using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Api.Receipts.Import.Photo;

/// <summary>Reads the text on a photo.</summary>
internal interface IReceiptOcr
{
    Task<Result<IReadOnlyList<OcrText>>> ReadAsync(Stream photo, string contentType, CancellationToken cancellationToken);
}

/// <summary>The "Ocr" section of appsettings.</summary>
internal sealed class OcrOptions
{
    public const string Section = "Ocr";

    /// <summary>Address of the OCR service (the <c>ocr</c> folder of the repository). Without it photos cannot be imported.</summary>
    public Uri? Url { get; set; }

    /// <summary>A large photo takes a while on a Raspberry Pi.</summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(2);
}

/// <summary>Sends the photo to the OCR service running next to the API.</summary>
internal sealed class HttpReceiptOcr(HttpClient http, ILogger<HttpReceiptOcr> logger) : IReceiptOcr
{
    public async Task<Result<IReadOnlyList<OcrText>>> ReadAsync(Stream photo, string contentType, CancellationToken cancellationToken)
    {
        using StreamContent content = new(photo);
        content.Headers.ContentType = MediaTypeHeaderValue.TryParse(contentType, out MediaTypeHeaderValue? type)
            ? type
            : new MediaTypeHeaderValue("application/octet-stream");

        try
        {
            using HttpResponseMessage response = await http.PostAsync(new Uri("ocr", UriKind.Relative), content, cancellationToken);
            if (response.StatusCode == HttpStatusCode.UnsupportedMediaType)
            {
                return Fail("Import.NotAnImage", "The file is not a photo.");
            }

            response.EnsureSuccessStatusCode();
            OcrResponse? read = await response.Content.ReadFromJsonAsync<OcrResponse>(cancellationToken);
            return Result.Success(read?.Texts() ?? []);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException { InnerException: TimeoutException })
        {
            logger.OcrFailed(exception);
            return Fail("Import.OcrUnavailable", "The text recognition service is not available.");
        }
    }

    private static Result<IReadOnlyList<OcrText>> Fail(string code, string message) =>
        Result.Failure<IReadOnlyList<OcrText>>(new Error(code, message, Target: "file"));
}

/// <summary>What the OCR service answers: each piece of text with the four corners of its box.</summary>
internal sealed record OcrResponse(IReadOnlyList<OcrResponse.Line>? Lines)
{
    public IReadOnlyList<OcrText> Texts() => [.. (Lines ?? []).Where(IsBox).Select(ToText)];

    private static bool IsBox(Line line) => line.Box is { Count: 4 } box && box.All(point => point.Count == 2);

    private static OcrText ToText(Line line)
    {
        OcrPoint[] corners = [.. line.Box!.Select(point => new OcrPoint(point[0], point[1]))];
        return new OcrText(line.Text ?? "", corners[0], corners[1], corners[2], corners[3]);
    }

    internal sealed record Line(string? Text, IReadOnlyList<IReadOnlyList<double>>? Box);
}

/// <summary>Used when no OCR service is configured, so the rest of the app works without it.</summary>
internal sealed class MissingReceiptOcr : IReceiptOcr
{
    public Task<Result<IReadOnlyList<OcrText>>> ReadAsync(Stream photo, string contentType, CancellationToken cancellationToken) =>
        Task.FromResult(Result.Failure<IReadOnlyList<OcrText>>(
            new Error("Import.OcrUnavailable", "The text recognition service is not configured.", Target: "file")));
}

internal static class ReceiptOcrSetup
{
    public static void AddReceiptOcr(this WebApplicationBuilder builder)
    {
        OcrOptions options = builder.Configuration.GetSection(OcrOptions.Section).Get<OcrOptions>() ?? new OcrOptions();
        if (options.Url is null)
        {
            builder.Services.AddSingleton<IReceiptOcr, MissingReceiptOcr>();
            return;
        }

        builder.Services.AddHttpClient<IReceiptOcr, HttpReceiptOcr>(http =>
        {
            http.BaseAddress = options.Url;
            http.Timeout = options.Timeout;
        });
    }
}
