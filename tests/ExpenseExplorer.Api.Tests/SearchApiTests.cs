using ExpenseExplorer.Contracts.Dictionaries;
using ExpenseExplorer.Contracts.ReceiptItems;
using ExpenseExplorer.Contracts.Receipts;

namespace ExpenseExplorer.Api.Tests;

/// <summary>Every search ignores Polish diacritics and, when nothing contains the text, tolerates typos.</summary>
public class SearchApiTests(ApiFixture api)
{
    private readonly HttpClient _client = api.Editor;

    [Fact]
    public async Task Suggestions_ignore_polish_diacritics_both_ways()
    {
        string tag = Tag();
        string item = $"Żółty ser {tag}";
        await _client.ReceiptAsync(Seed.UniqueStore("Lidl"), new DateOnly(2026, 9, 1), Line(item, $"Nabiał {tag}"));

        Assert.Equal([item], await Suggestions("items", $"zolty ser {tag}"));
        Assert.Equal([$"Nabiał {tag}"], await Suggestions("categories", $"NABIAL {tag}"));
        Assert.Equal([item], await Suggestions("items", $"ŻÓŁTY SER {tag}"));
    }

    [Fact]
    public async Task A_typo_finds_the_most_similar_names_first()
    {
        string tag = Tag();
        string store = $"Biedronka {tag}";
        await _client.ReceiptAsync(store, new DateOnly(2026, 9, 1));

        IReadOnlyList<string> found = await Suggestions("stores", $"biedronak {tag}");

        Assert.Equal(store, found[0]);
    }

    [Fact]
    public async Task Similar_names_are_left_out_when_some_name_contains_the_search()
    {
        string tag = Tag();
        await _client.ReceiptAsync($"Kaufland {tag}", new DateOnly(2026, 9, 1));
        await _client.ReceiptAsync($"Kaufladn {tag}", new DateOnly(2026, 9, 1));

        Assert.Equal([$"Kaufland {tag}"], await Suggestions("stores", $"kaufland {tag}"));
    }

    [Fact]
    public async Task Searches_shorter_than_three_letters_do_not_guess()
    {
        Assert.Empty(await Suggestions("stores", "qx"));
    }

    [Fact]
    public async Task Dictionary_screen_finds_names_despite_typos_and_missing_diacritics()
    {
        string tag = Tag();
        string item = $"Śmietana {tag}";
        await _client.ReceiptAsync(Seed.UniqueStore("Lidl"), new DateOnly(2026, 9, 1), Line(item, "Nabiał"));

        Assert.Equal(item, (await Usage("items", $"smietana {tag}"))[0].Name);
        Assert.Equal(item, (await Usage("items", $"smeitana {tag}"))[0].Name);
    }

    [Fact]
    public async Task Description_filter_ignores_diacritics_and_tolerates_typos()
    {
        string tag = Tag();
        string store = Seed.UniqueStore("Żabka");
        await _client.ReceiptAsync(
            store,
            new DateOnly(2026, 9, 1),
            new ReceiptItemRequest("Mleko", "Nabiał", 1m, 3.49m, null, $"Promocja świąteczna {tag}"),
            new ReceiptItemRequest("Chleb", "Pieczywo", 1m, 4.99m, null, null));

        Assert.Equal(["Mleko"], await LinesWithDescription(store, $"swiateczna {tag}"));
        Assert.Equal(["Mleko"], await LinesWithDescription(store, $"promcoja {tag}"));
    }

    /// <summary>A random word so that names left by other tests never match.</summary>
    private static string Tag() => Guid.NewGuid().ToString("N")[..12];

    private static ReceiptItemRequest Line(string item, string category) => new(item, category, 1m, 10m, null, null);

    private async Task<IReadOnlyList<string>> Suggestions(string kind, string search) =>
        await (await _client.Get($"/api/v1/{kind}?search={Uri.EscapeDataString(search)}")).Read<List<string>>();

    private async Task<IReadOnlyList<NameUsageResponse>> Usage(string kind, string search) =>
        await (await _client.Get($"/api/v1/dictionaries/{kind}?search={Uri.EscapeDataString(search)}")).Read<List<NameUsageResponse>>();

    private async Task<IReadOnlyList<string>> LinesWithDescription(string store, string description)
    {
        ReceiptItemListResponse list = await (await _client.Get(
                $"/api/v1/receipt-items?stores={Uri.EscapeDataString(store)}&description={Uri.EscapeDataString(description)}"))
            .Read<ReceiptItemListResponse>();
        return [.. list.Items.Items.Select(line => line.Item)];
    }
}
