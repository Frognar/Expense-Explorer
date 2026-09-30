using ExpenseExplorer.Contracts.ReceiptItems;
using ExpenseExplorer.Contracts.Receipts;

namespace ExpenseExplorer.Web.Localization;

/// <summary>Every text of the interface. A missing translation is a compile error, not a blank label.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "S2068", Justification = "Labels of the sign-in form, not credentials.")]
public sealed class UiText
{
    public required string Receipts { get; init; }
    public required string Items { get; init; }
    public required string Reports { get; init; }
    public required string SignIn { get; init; }
    public required string SignOut { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
    public required string ReadOnlyAccess { get; init; }
    public required string Save { get; init; }
    public required string Cancel { get; init; }
    public required string Delete { get; init; }
    public required string Edit { get; init; }
    public required string Filters { get; init; }
    public required string Apply { get; init; }
    public required string ClearFilters { get; init; }
    public required string SortBy { get; init; }
    public required string Ascending { get; init; }
    public required string Descending { get; init; }
    public required string NothingFound { get; init; }
    public required string Total { get; init; }
    public required string TotalOfFiltered { get; init; }
    public required string From { get; init; }
    public required string To { get; init; }
    public required string Min { get; init; }
    public required string Max { get; init; }
    public required string Store { get; init; }
    public required string Stores { get; init; }
    public required string PurchaseDate { get; init; }
    public required string Item { get; init; }
    public required string ItemNames { get; init; }
    public required string Category { get; init; }
    public required string Categories { get; init; }
    public required string Quantity { get; init; }
    public required string UnitPrice { get; init; }
    public required string Amount { get; init; }
    public required string AmountHelp { get; init; }
    public required string Discount { get; init; }
    public required string Description { get; init; }
    public required string DescriptionContains { get; init; }
    public required string NewReceipt { get; init; }
    public required string EditReceipt { get; init; }
    public required string ImportBiedronka { get; init; }
    public required string Imported { get; init; }
    public required string AddItem { get; init; }
    public required string EditItem { get; init; }
    public required string NoItemsYet { get; init; }
    public required string Duplicate { get; init; }
    public required string DuplicateAsOf { get; init; }
    public required string ExportCsv { get; init; }
    public required string DeleteReceipt { get; init; }
    public required string DeleteReceiptQuestion { get; init; }
    public required string DeleteItemQuestion { get; init; }
    public required string Saved { get; init; }
    public required string Deleted { get; init; }
    public required string CategoryReport { get; init; }
    public required string ShowItems { get; init; }
    public required string NotFound { get; init; }
    public required string UnexpectedError { get; init; }
    public required string InvalidNumber { get; init; }
    public required IReadOnlyDictionary<ReceiptSortField, string> ReceiptSort { get; init; }
    public required IReadOnlyDictionary<ReceiptItemSortField, string> ItemSort { get; init; }
    public required IReadOnlyDictionary<string, string> Errors { get; init; }

    public static readonly UiText Polish = new()
    {
        Receipts = "Paragony",
        Items = "Pozycje",
        Reports = "Raporty",
        SignIn = "Zaloguj",
        SignOut = "Wyloguj",
        UserName = "Użytkownik",
        Password = "Hasło",
        ReadOnlyAccess = "Tylko odczyt",
        Save = "Zapisz",
        Cancel = "Anuluj",
        Delete = "Usuń",
        Edit = "Edytuj",
        Filters = "Filtry",
        Apply = "Zastosuj",
        ClearFilters = "Wyczyść filtry",
        SortBy = "Sortuj",
        Ascending = "Rosnąco",
        Descending = "Malejąco",
        NothingFound = "Nic nie znaleziono.",
        Total = "Suma",
        TotalOfFiltered = "Suma dla filtra",
        From = "Od",
        To = "Do",
        Min = "od",
        Max = "do",
        Store = "Sklep",
        Stores = "Sklepy",
        PurchaseDate = "Data zakupu",
        Item = "Towar",
        ItemNames = "Towary",
        Category = "Kategoria",
        Categories = "Kategorie",
        Quantity = "Ilość",
        UnitPrice = "Cena jedn.",
        Amount = "Kwota",
        AmountHelp = "Za całą pozycję, przed rabatem",
        Discount = "Rabat",
        Description = "Opis",
        DescriptionContains = "Opis zawiera",
        NewReceipt = "Nowy paragon",
        EditReceipt = "Edytuj paragon",
        ImportBiedronka = "Import z Biedronki",
        Imported = "Paragon zaimportowany.",
        AddItem = "Dodaj pozycję",
        EditItem = "Edytuj pozycję",
        NoItemsYet = "Paragon nie ma jeszcze pozycji.",
        Duplicate = "Duplikuj",
        DuplicateAsOf = "Duplikuj z datą",
        ExportCsv = "Eksport CSV",
        DeleteReceipt = "Usuń paragon",
        DeleteReceiptQuestion = "Usunąć paragon razem ze wszystkimi pozycjami?",
        DeleteItemQuestion = "Usunąć tę pozycję?",
        Saved = "Zapisano.",
        Deleted = "Usunięto.",
        CategoryReport = "Wydatki według kategorii",
        ShowItems = "Pokaż pozycje",
        NotFound = "Nie ma takiej strony.",
        UnexpectedError = "Coś poszło nie tak. Spróbuj ponownie.",
        InvalidNumber = "To nie jest liczba.",
        ReceiptSort = new Dictionary<ReceiptSortField, string>
        {
            [ReceiptSortField.PurchaseDate] = "Data",
            [ReceiptSortField.Store] = "Sklep",
            [ReceiptSortField.Total] = "Suma",
        },
        ItemSort = new Dictionary<ReceiptItemSortField, string>
        {
            [ReceiptItemSortField.PurchaseDate] = "Data",
            [ReceiptItemSortField.Store] = "Sklep",
            [ReceiptItemSortField.Item] = "Towar",
            [ReceiptItemSortField.Category] = "Kategoria",
            [ReceiptItemSortField.Quantity] = "Ilość",
            [ReceiptItemSortField.UnitPrice] = "Cena jedn.",
            [ReceiptItemSortField.Amount] = "Kwota",
            [ReceiptItemSortField.Discount] = "Rabat",
            [ReceiptItemSortField.Total] = "Suma",
            [ReceiptItemSortField.Description] = "Opis",
        },
        Errors = new Dictionary<string, string>
        {
            ["Input.Required"] = "To pole jest wymagane.",
            ["Input.OutOfRange"] = "Wartość jest poza dozwolonym zakresem.",
            ["Input.RangeReversed"] = "Początek zakresu jest po jego końcu.",
            ["StoreName.Empty"] = "Podaj sklep.",
            ["StoreName.TooLong"] = "Nazwa sklepu może mieć najwyżej 100 znaków.",
            ["ItemName.Empty"] = "Podaj towar.",
            ["ItemName.TooLong"] = "Nazwa towaru może mieć najwyżej 100 znaków.",
            ["CategoryName.Empty"] = "Podaj kategorię.",
            ["CategoryName.TooLong"] = "Kategoria może mieć najwyżej 100 znaków.",
            ["Description.TooLong"] = "Opis może mieć najwyżej 500 znaków.",
            ["PurchaseDate.InFuture"] = "Data zakupu nie może być z przyszłości.",
            ["Money.Negative"] = "Kwota nie może być ujemna.",
            ["Money.TooPrecise"] = "Kwota może mieć najwyżej 2 miejsca po przecinku.",
            ["Quantity.NotPositive"] = "Ilość musi być większa od zera.",
            ["Quantity.TooPrecise"] = "Ilość może mieć najwyżej 4 miejsca po przecinku.",
            ["LinePrice.DiscountExceedsAmount"] = "Rabat nie może być większy niż kwota.",
            ["Receipt.NotFound"] = "Nie ma takiego paragonu.",
            ["Receipt.ItemNotFound"] = "Nie ma takiej pozycji.",
            ["Receipt.ItemAlreadyExists"] = "Ta pozycja już jest na paragonie.",
            ["ReceiptId.Empty"] = "Nie ma takiego paragonu.",
            ["ReceiptItemId.Empty"] = "Nie ma takiej pozycji.",
            ["Auth.InvalidCredentials"] = "Nieprawidłowy użytkownik lub hasło.",
            ["Auth.SessionExpired"] = "Sesja wygasła. Zaloguj się ponownie.",
            ["Import.EmptyFile"] = "Plik jest pusty.",
            ["Import.FileTooLarge"] = "Plik jest większy niż 1 MB.",
            ["Import.InvalidJson"] = "To nie jest plik paragonu (niepoprawny JSON).",
            ["Import.MissingDate"] = "W pliku brakuje daty zakupu.",
            ["Import.MissingBody"] = "W pliku brakuje pozycji paragonu.",
            ["Import.NoItems"] = "Paragon nie ma żadnych pozycji.",
            ["Import.Storno"] = "Paragony ze stornem nie są obsługiwane.",
            ["Import.Surcharge"] = "Dopłaty na paragonie nie są obsługiwane.",
            ["Import.DiscountWithoutItem"] = "Rabat pojawia się przed pierwszą pozycją.",
            ["Import.InvalidQuantity"] = "Pozycja ma niepoprawną ilość.",
            ["Import.InvalidPrice"] = "Pozycja nie ma ceny.",
            ["Import.InvalidAmount"] = "Plik zawiera niepoprawną kwotę.",
            ["Import.InvalidDiscount"] = "Plik zawiera niepoprawny rabat.",
            ["Import.VoucherTooLarge"] = "Bon jest większy niż wartość paragonu.",
            ["Http.401"] = "Zaloguj się ponownie.",
            ["Http.403"] = "Nie masz uprawnień do tej operacji.",
            ["Http.404"] = "Nie znaleziono.",
            ["Http.429"] = "Za dużo prób. Odczekaj chwilę.",
        },
    };

    public static readonly UiText English = new()
    {
        Receipts = "Receipts",
        Items = "Items",
        Reports = "Reports",
        SignIn = "Sign in",
        SignOut = "Sign out",
        UserName = "User name",
        Password = "Password",
        ReadOnlyAccess = "Read only",
        Save = "Save",
        Cancel = "Cancel",
        Delete = "Delete",
        Edit = "Edit",
        Filters = "Filters",
        Apply = "Apply",
        ClearFilters = "Clear filters",
        SortBy = "Sort",
        Ascending = "Ascending",
        Descending = "Descending",
        NothingFound = "Nothing found.",
        Total = "Total",
        TotalOfFiltered = "Total for filter",
        From = "From",
        To = "To",
        Min = "min",
        Max = "max",
        Store = "Store",
        Stores = "Stores",
        PurchaseDate = "Purchase date",
        Item = "Item",
        ItemNames = "Items",
        Category = "Category",
        Categories = "Categories",
        Quantity = "Quantity",
        UnitPrice = "Unit price",
        Amount = "Amount",
        AmountHelp = "For the whole line, before discount",
        Discount = "Discount",
        Description = "Description",
        DescriptionContains = "Description contains",
        NewReceipt = "New receipt",
        EditReceipt = "Edit receipt",
        ImportBiedronka = "Import from Biedronka",
        Imported = "Receipt imported.",
        AddItem = "Add item",
        EditItem = "Edit item",
        NoItemsYet = "This receipt has no items yet.",
        Duplicate = "Duplicate",
        DuplicateAsOf = "Duplicate with date",
        ExportCsv = "Export CSV",
        DeleteReceipt = "Delete receipt",
        DeleteReceiptQuestion = "Delete the receipt with all its items?",
        DeleteItemQuestion = "Delete this item?",
        Saved = "Saved.",
        Deleted = "Deleted.",
        CategoryReport = "Spending by category",
        ShowItems = "Show items",
        NotFound = "There is no such page.",
        UnexpectedError = "Something went wrong. Please try again.",
        InvalidNumber = "This is not a number.",
        ReceiptSort = new Dictionary<ReceiptSortField, string>
        {
            [ReceiptSortField.PurchaseDate] = "Date",
            [ReceiptSortField.Store] = "Store",
            [ReceiptSortField.Total] = "Total",
        },
        ItemSort = new Dictionary<ReceiptItemSortField, string>
        {
            [ReceiptItemSortField.PurchaseDate] = "Date",
            [ReceiptItemSortField.Store] = "Store",
            [ReceiptItemSortField.Item] = "Item",
            [ReceiptItemSortField.Category] = "Category",
            [ReceiptItemSortField.Quantity] = "Quantity",
            [ReceiptItemSortField.UnitPrice] = "Unit price",
            [ReceiptItemSortField.Amount] = "Amount",
            [ReceiptItemSortField.Discount] = "Discount",
            [ReceiptItemSortField.Total] = "Total",
            [ReceiptItemSortField.Description] = "Description",
        },
        Errors = new Dictionary<string, string>
        {
            ["Input.Required"] = "This field is required.",
            ["Input.OutOfRange"] = "The value is out of range.",
            ["Input.RangeReversed"] = "The start of the range is after its end.",
            ["StoreName.Empty"] = "Enter the store.",
            ["StoreName.TooLong"] = "The store name can have at most 100 characters.",
            ["ItemName.Empty"] = "Enter the item.",
            ["ItemName.TooLong"] = "The item name can have at most 100 characters.",
            ["CategoryName.Empty"] = "Enter the category.",
            ["CategoryName.TooLong"] = "The category can have at most 100 characters.",
            ["Description.TooLong"] = "The description can have at most 500 characters.",
            ["PurchaseDate.InFuture"] = "The purchase date cannot be in the future.",
            ["Money.Negative"] = "The amount cannot be negative.",
            ["Money.TooPrecise"] = "The amount can have at most 2 decimal places.",
            ["Quantity.NotPositive"] = "The quantity must be greater than zero.",
            ["Quantity.TooPrecise"] = "The quantity can have at most 4 decimal places.",
            ["LinePrice.DiscountExceedsAmount"] = "The discount cannot be larger than the amount.",
            ["Receipt.NotFound"] = "There is no such receipt.",
            ["Receipt.ItemNotFound"] = "There is no such item.",
            ["Receipt.ItemAlreadyExists"] = "This item is already on the receipt.",
            ["ReceiptId.Empty"] = "There is no such receipt.",
            ["ReceiptItemId.Empty"] = "There is no such item.",
            ["Auth.InvalidCredentials"] = "Wrong user name or password.",
            ["Auth.SessionExpired"] = "Your session has expired. Please sign in again.",
            ["Import.EmptyFile"] = "The file is empty.",
            ["Import.FileTooLarge"] = "The file is larger than 1 MB.",
            ["Import.InvalidJson"] = "This is not a receipt file (invalid JSON).",
            ["Import.MissingDate"] = "The file has no purchase date.",
            ["Import.MissingBody"] = "The file has no receipt lines.",
            ["Import.NoItems"] = "The receipt has no items.",
            ["Import.Storno"] = "Receipts with cancelled lines are not supported.",
            ["Import.Surcharge"] = "Surcharges are not supported.",
            ["Import.DiscountWithoutItem"] = "A discount comes before the first item.",
            ["Import.InvalidQuantity"] = "An item has an invalid quantity.",
            ["Import.InvalidPrice"] = "An item has no price.",
            ["Import.InvalidAmount"] = "The file contains an invalid amount.",
            ["Import.InvalidDiscount"] = "The file contains an invalid discount.",
            ["Import.VoucherTooLarge"] = "The voucher is larger than the receipt.",
            ["Http.401"] = "Please sign in again.",
            ["Http.403"] = "You are not allowed to do this.",
            ["Http.404"] = "Not found.",
            ["Http.429"] = "Too many attempts. Please wait a moment.",
        },
    };
}
