using System.Net;
using System.Net.Http.Headers;
using System.Text;
using ExpenseExplorer.Contracts.Dictionaries;
using ExpenseExplorer.Contracts.Receipts;

namespace ExpenseExplorer.Api.Tests;

public class DictionaryApiTests(ApiFixture api)
{
    [Fact]
    public async Task Renaming_an_item_to_a_name_in_use_merges_them()
    {
        string store = Seed.UniqueStore("Merge");
        string spelledOut = Unique("Mleko 3,2%");
        string printed = Unique("MLEK WYPAS");
        await api.Editor.ReceiptAsync(store, new DateOnly(2026, 9, 1), Line(spelledOut, "Nabiał"));
        ReceiptResponse later = await api.Editor.ReceiptAsync(store, new DateOnly(2026, 9, 2), Line(printed, "Nabiał"), Line(printed, "Nabiał"));

        HttpResponseMessage response = await Rename(api.Editor, "items", printed, spelledOut);
        RenameNameResponse renamed = await response.Read<RenameNameResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(new RenameNameResponse(spelledOut, 2, Merged: true), renamed);
        ReceiptResponse receipt = await (await api.Editor.Get($"/api/v1/receipts/{later.Id}")).Read<ReceiptResponse>();
        Assert.All(receipt.Items, item => Assert.Equal(spelledOut, item.Item));
        Assert.Equal(
            [new NameUsageResponse(spelledOut, 3, new DateOnly(2026, 9, 2))],
            await Usage(api.Editor, "items", spelledOut));
    }

    [Fact]
    public async Task Renaming_a_store_changes_every_receipt_from_it()
    {
        string store = Seed.UniqueStore("Old");
        string newName = Seed.UniqueStore("New");
        await api.Editor.ReceiptAsync(store, new DateOnly(2026, 9, 1));
        await api.Editor.ReceiptAsync(store, new DateOnly(2026, 9, 3));

        RenameNameResponse renamed = await (await Rename(api.Editor, "stores", store, newName)).Read<RenameNameResponse>();

        Assert.Equal(new RenameNameResponse(newName, 2, Merged: false), renamed);
        Assert.Empty(await Usage(api.Editor, "stores", store));
        Assert.Equal([new NameUsageResponse(newName, 2, new DateOnly(2026, 9, 3))], await Usage(api.Editor, "stores", newName));
    }

    [Fact]
    public async Task Import_gives_an_item_the_category_it_had_last_time()
    {
        string item = Unique("Kawa ziarnista");
        await api.Editor.ReceiptAsync(Seed.UniqueStore("Earlier"), new DateOnly(2026, 8, 1), Line(item, "Napoje"));
        await api.Editor.ReceiptAsync(Seed.UniqueStore("Earlier"), new DateOnly(2026, 9, 1), Line(item, "Kawa"));

        ReceiptResponse imported = await Import(item, "Nowość " + Guid.NewGuid().ToString("N")[..8]);

        Assert.Equal([(item, "Kawa"), (imported.Items[1].Item, "Spożywcze")], imported.Items.Select(line => (line.Item, line.Category)));
    }

    [Fact]
    public async Task Import_uses_the_new_name_of_a_renamed_item_and_its_category()
    {
        string printed = Unique("SER GOUDA PLAST");
        string renamed = Unique("Ser gouda");
        await api.Editor.ReceiptAsync(Seed.UniqueStore("Earlier"), new DateOnly(2026, 9, 1), Line(printed, "Nabiał"));
        (await Rename(api.Editor, "items", printed, renamed)).EnsureSuccessStatusCode();

        // Receipts print names in capitals or not, so the old name matches either way.
        ReceiptResponse imported = await Import(printed.ToUpperInvariant());

        Assert.Equal((renamed, "Nabiał"), (imported.Items[0].Item, imported.Items[0].Category));
    }

    [Fact]
    public async Task Names_renamed_twice_lead_to_the_latest_name()
    {
        string first = Unique("A");
        string second = Unique("B");
        string third = Unique("C");
        await api.Editor.ReceiptAsync(Seed.UniqueStore("Chain"), new DateOnly(2026, 9, 1), Line(first, "Chemia"));
        (await Rename(api.Editor, "items", first, second)).EnsureSuccessStatusCode();
        (await Rename(api.Editor, "items", second, third)).EnsureSuccessStatusCode();

        ReceiptResponse imported = await Import(first);

        Assert.Equal((third, "Chemia"), (imported.Items[0].Item, imported.Items[0].Category));
    }

    [Fact]
    public async Task Name_given_back_to_a_renamed_item_is_its_own_again()
    {
        string first = Unique("A");
        string second = Unique("B");
        await api.Editor.ReceiptAsync(Seed.UniqueStore("Back"), new DateOnly(2026, 9, 1), Line(first, "Chemia"));
        (await Rename(api.Editor, "items", first, second)).EnsureSuccessStatusCode();
        (await Rename(api.Editor, "items", second, first)).EnsureSuccessStatusCode();

        ReceiptResponse imported = await Import(first, second);

        Assert.Equal([first, first], imported.Items.Select(line => line.Item));
    }

    [Theory]
    [InlineData("", "x", "from", "Input.Required")]
    [InlineData("x", " ", "to", "ItemName.Empty")]
    [InlineData("x", "x", "to", "Dictionary.SameName")]
    public async Task Invalid_rename_is_rejected(string from, string to, string field, string code)
    {
        HttpResponseMessage response = await Rename(api.Editor, "items", from, to);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(code, (await response.ErrorCodes())[field]);
    }

    [Fact]
    public async Task Renaming_a_name_no_receipt_uses_is_not_found()
    {
        HttpResponseMessage response = await Rename(api.Editor, "categories", Unique("Nothing"), "Inne");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Dictionary.NameNotFound", (await response.ErrorCodes())["from"]);
    }

    [Fact]
    public async Task Unknown_dictionary_is_not_found()
    {
        Assert.Equal(HttpStatusCode.NotFound, (await api.Editor.Get("/api/v1/dictionaries/colours")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await Rename(api.Editor, "colours", "a", "b")).StatusCode);
    }

    [Fact]
    public async Task Reader_can_look_but_not_rename()
    {
        Assert.Equal(HttpStatusCode.OK, (await api.Reader.Get("/api/v1/dictionaries/categories")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Rename(api.Reader, "categories", "a", "b")).StatusCode);
    }

    private static string Unique(string name) => $"{name} {Guid.NewGuid():N}"[..(name.Length + 13)];

    private static ReceiptItemRequest Line(string item, string category) => new(item, category, 1m, 10m, null, null);

    private static Task<HttpResponseMessage> Rename(HttpClient client, string kind, string from, string to) =>
        client.Post($"/api/v1/dictionaries/{kind}/rename", new RenameNameRequest(from, to));

    private static async Task<IReadOnlyList<NameUsageResponse>> Usage(HttpClient client, string kind, string search) =>
        await (await client.Get($"/api/v1/dictionaries/{kind}?search={Uri.EscapeDataString(search)}")).Read<List<NameUsageResponse>>();

    /// <summary>A Biedronka e-receipt with one line per name, imported by the editor.</summary>
    private async Task<ReceiptResponse> Import(params string[] items)
    {
        string lines = string.Join(
            ",",
            items.Select(item => $$"""{ "sellLine": { "name": "{{item}}", "quantity": "1", "price": 500 } }"""));
        string json = $$"""{ "header": [ { "headerData": { "date": "2026-09-28T10:00:00Z" } } ], "body": [ {{lines}} ] }""";

        using MultipartFormDataContent form = new();
        using ByteArrayContent file = new(Encoding.UTF8.GetBytes(json));
        file.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        form.Add(file, "file", "paragon.json");
        HttpResponseMessage response = await api.Editor.PostAsync(
            new Uri("/api/v1/receipts/import/biedronka", UriKind.Relative), form, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await response.Read<ReceiptResponse>();
    }
}
