using ExpenseExplorer.Contracts.Logs;
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
    public required string DarkTheme { get; init; }
    public required string LightTheme { get; init; }
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
    public required string ImportPhoto { get; init; }
    public required string ReadingPhoto { get; init; }
    public required string PhotoTotalMatches { get; init; }
    public required Func<string, string, string> PhotoTotalDiffers { get; init; }
    public required string PhotoTotalMissing { get; init; }
    public required string PhotoDateMissing { get; init; }
    public required string PhotoSkippedLines { get; init; }
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
    public required string Logs { get; init; }
    public required string Day { get; init; }
    public required string LevelFrom { get; init; }
    public required string Search { get; init; }
    public required string Refresh { get; init; }
    public required string Source { get; init; }
    public required string Exception { get; init; }
    public required string Dictionaries { get; init; }
    public required string Rename { get; init; }
    public required string NewName { get; init; }
    public required string RenameHelp { get; init; }
    public required Func<int, string, string> UsedTimes { get; init; }
    public required Func<int, string> ChangedCount { get; init; }
    public required Func<string, string> MergedInto { get; init; }
    public required Func<int, string> ShowingFirst { get; init; }
    public required string Budget { get; init; }
    public required string FreePool { get; init; }
    public required string PerDay { get; init; }
    public required Func<int, string> DaysLeft { get; init; }
    public required string Funds { get; init; }
    public required string Planned { get; init; }
    public required string Spent { get; init; }
    public required Func<string, string, string> SpentOf { get; init; }
    public required Func<string, string> OverPlanBy { get; init; }
    public required Func<string, string> LeftInGroup { get; init; }
    public required string OutsideGroups { get; init; }
    public required string AddFund { get; init; }
    public required string EditFund { get; init; }
    public required string AddPlanItem { get; init; }
    public required string EditPlanItem { get; init; }
    public required string Name { get; init; }
    public required string FundAmountHelp { get; init; }
    public required string Estimate { get; init; }
    public required string DayOfMonth { get; init; }
    public required string Group { get; init; }
    public required string NoCurrentPeriod { get; init; }
    public required string NewPeriod { get; init; }
    public required string PeriodStart { get; init; }
    public required string NewPeriodHelp { get; init; }
    public required string DeleteQuestion { get; init; }
    public required string NoGroupsYet { get; init; }
    public required string BudgetSettings { get; init; }
    public required string Groups { get; init; }
    public required string AddGroup { get; init; }
    public required string RenameGroup { get; init; }
    public required string DeleteGroupQuestion { get; init; }
    public required string CategoriesHelp { get; init; }
    public required string NoGroup { get; init; }
    public required string Template { get; init; }
    public required string TemplateHelp { get; init; }
    public required string LoadTemplate { get; init; }
    public required string SaveAsTemplate { get; init; }
    public required string TemplateSaved { get; init; }
    public required string InvalidTemplateFile { get; init; }
    public required IReadOnlyDictionary<LogSeverity, string> LogLevels { get; init; }
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
        DarkTheme = "Ciemny motyw",
        LightTheme = "Jasny motyw",
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
        ImportPhoto = "Ze zdjęcia",
        ReadingPhoto = "Odczytuję paragon ze zdjęcia…",
        PhotoTotalMatches = "Paragon zaimportowany, suma zgadza się z paragonem.",
        PhotoTotalDiffers = (lines, printed) => $"Suma pozycji {lines} różni się od sumy na paragonie {printed}. Sprawdź pozycje.",
        PhotoTotalMissing = "Nie odczytałem sumy z paragonu. Sprawdź pozycje.",
        PhotoDateMissing = "Nie odczytałem daty zakupu, ustawiłem dzisiejszą.",
        PhotoSkippedLines = "Nie odczytałem tych linii:",
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
        Logs = "Logi",
        Day = "Dzień",
        LevelFrom = "Poziom od",
        Search = "Szukaj",
        Refresh = "Odśwież",
        Source = "Źródło",
        Exception = "Wyjątek",
        Dictionaries = "Słowniki",
        Rename = "Zmień nazwę",
        NewName = "Nowa nazwa",
        RenameHelp = "Zmiana obejmie wszystkie paragony. Jeśli nowa nazwa już istnieje, obie zostaną połączone. Import, który odczyta starą nazwę, wpisze nową.",
        UsedTimes = (count, last) => $"{count}× · ostatnio {last}",
        ChangedCount = count => $"Zmieniono: {count}.",
        MergedInto = name => $"Połączono z „{name}”.",
        ShowingFirst = count => $"Pokazuję pierwsze {count}. Zawęź wyszukiwanie.",
        Budget = "Budżet",
        FreePool = "Wolna pula",
        PerDay = "Na dzień",
        DaysLeft = days => days switch { 0 => "Okres się zakończył", 1 => "Ostatni dzień okresu", _ => $"{days} dni do końca okresu" },
        Funds = "Środki",
        Planned = "Zaplanowane",
        Spent = "Wydane",
        SpentOf = (spent, planned) => $"{spent} z {planned}",
        OverPlanBy = amount => $"ponad plan o {amount}",
        LeftInGroup = amount => $"zostało {amount}",
        OutsideGroups = "Poza grupami",
        AddFund = "Dodaj korektę",
        EditFund = "Edytuj środki",
        AddPlanItem = "Dodaj wydatek",
        EditPlanItem = "Edytuj wydatek",
        Name = "Nazwa",
        FundAmountHelp = "Odejmij od środków, np. oszczędności albo mniejszy przychód",
        Estimate = "Zakładana",
        DayOfMonth = "Dzień miesiąca",
        Group = "Grupa",
        NoCurrentPeriod = "Żaden okres budżetu nie obejmuje dzisiejszego dnia.",
        NewPeriod = "Nowy okres",
        PeriodStart = "Początek okresu",
        NewPeriodHelp = "Okres potrwa miesiąc i dostanie przychody i wydatki z szablonu.",
        DeleteQuestion = "Usunąć tę pozycję?",
        NoGroupsYet = "Nie ma jeszcze grup budżetu. Dodasz je w ustawieniach budżetu.",
        BudgetSettings = "Ustawienia budżetu",
        Groups = "Grupy",
        AddGroup = "Dodaj grupę",
        RenameGroup = "Zmień nazwę grupy",
        DeleteGroupQuestion = "Usunąć tę grupę? Jej kategorie trafią poza grupy.",
        CategoriesHelp = "Wydatki z paragonów liczą się do grupy, do której należy ich kategoria.",
        NoGroup = "Poza grupami",
        Template = "Szablon",
        TemplateHelp = "Każdy nowy okres dostaje te przychody i wydatki.",
        LoadTemplate = "Wczytaj z pliku JSON",
        SaveAsTemplate = "Zapisz ten okres jako szablon",
        TemplateSaved = "Szablon zapisany.",
        InvalidTemplateFile = "To nie jest plik szablonu budżetu.",
        LogLevels = new Dictionary<LogSeverity, string>
        {
            [LogSeverity.Verbose] = "Wszystko",
            [LogSeverity.Debug] = "Debug",
            [LogSeverity.Information] = "Informacje",
            [LogSeverity.Warning] = "Ostrzeżenia",
            [LogSeverity.Error] = "Błędy",
            [LogSeverity.Fatal] = "Krytyczne",
        },
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
            ["Import.NothingRead"] = "Nie udało się odczytać żadnej pozycji ze zdjęcia.",
            ["Import.NotAnImage"] = "To nie jest zdjęcie.",
            ["Import.PhotoTooLarge"] = "Zdjęcie jest większe niż 20 MB.",
            ["Import.OcrUnavailable"] = "Odczytywanie zdjęć jest teraz niedostępne.",
            ["Dictionary.NameNotFound"] = "Ta nazwa nie jest już nigdzie używana.",
            ["Dictionary.SameName"] = "Nowa nazwa jest taka sama jak stara.",
            ["Dictionary.UnknownKind"] = "Nie ma takiego słownika.",
            ["BudgetName.Empty"] = "Podaj nazwę.",
            ["BudgetName.TooLong"] = "Nazwa może mieć najwyżej 100 znaków.",
            ["FundAmount.Zero"] = "Kwota nie może być zerem.",
            ["DayOfMonth.OutOfRange"] = "Dzień musi być od 1 do 31.",
            ["BudgetPeriod.EndBeforeStart"] = "Okres kończy się przed początkiem.",
            ["BudgetPeriod.TooLong"] = "Okres może trwać najwyżej 62 dni.",
            ["Budget.PeriodNotFound"] = "Nie ma takiego okresu budżetu.",
            ["Budget.NoCurrentPeriod"] = "Żaden okres budżetu nie obejmuje dzisiejszego dnia.",
            ["Budget.FundNotFound"] = "Nie ma takiej pozycji środków.",
            ["Budget.ItemNotFound"] = "Nie ma takiego wydatku.",
            ["Budget.GroupNotFound"] = "Nie ma takiej grupy.",
            ["Budget.GroupNameTaken"] = "Grupa o tej nazwie już istnieje.",
            ["Budget.GroupInUse"] = "Grupa ma jeszcze zaplanowane wydatki.",
            ["Budget.PeriodOverlaps"] = "Ten okres nachodzi na inny.",
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
        DarkTheme = "Dark theme",
        LightTheme = "Light theme",
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
        ImportPhoto = "From photo",
        ReadingPhoto = "Reading the receipt photo…",
        PhotoTotalMatches = "Receipt imported; the total matches the paper.",
        PhotoTotalDiffers = (lines, printed) => $"The items add up to {lines}, but the receipt says {printed}. Check the items.",
        PhotoTotalMissing = "Could not read the total on the receipt. Check the items.",
        PhotoDateMissing = "Could not read the purchase date; today is set.",
        PhotoSkippedLines = "Could not read these lines:",
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
        Logs = "Logs",
        Day = "Day",
        LevelFrom = "Level from",
        Search = "Search",
        Refresh = "Refresh",
        Source = "Source",
        Exception = "Exception",
        Dictionaries = "Dictionaries",
        Rename = "Rename",
        NewName = "New name",
        RenameHelp = "This changes every receipt. If the new name is already in use, the two are merged. Imports that read the old name will enter the new one.",
        UsedTimes = (count, last) => $"{count}× · last {last}",
        ChangedCount = count => $"Changed: {count}.",
        MergedInto = name => $"Merged into \"{name}\".",
        ShowingFirst = count => $"Showing the first {count}. Narrow the search.",
        Budget = "Budget",
        FreePool = "Free pool",
        PerDay = "Per day",
        DaysLeft = days => days switch { 0 => "The period is over", 1 => "Last day of the period", _ => $"{days} days left in the period" },
        Funds = "Funds",
        Planned = "Planned",
        Spent = "Spent",
        SpentOf = (spent, planned) => $"{spent} of {planned}",
        OverPlanBy = amount => $"{amount} over plan",
        LeftInGroup = amount => $"{amount} left",
        OutsideGroups = "Outside groups",
        AddFund = "Add change",
        EditFund = "Edit funds",
        AddPlanItem = "Add expense",
        EditPlanItem = "Edit expense",
        Name = "Name",
        FundAmountHelp = "Subtract from the funds, e.g. savings or a smaller income",
        Estimate = "Estimate",
        DayOfMonth = "Day of month",
        Group = "Group",
        NoCurrentPeriod = "No budget period covers today.",
        NewPeriod = "New period",
        PeriodStart = "Period start",
        NewPeriodHelp = "The period lasts a month and gets the incomes and expenses of the template.",
        DeleteQuestion = "Delete this entry?",
        NoGroupsYet = "There are no budget groups yet. Add them in the budget settings.",
        BudgetSettings = "Budget settings",
        Groups = "Groups",
        AddGroup = "Add group",
        RenameGroup = "Rename group",
        DeleteGroupQuestion = "Delete this group? Its categories will be outside groups.",
        CategoriesHelp = "Spending from receipts counts in the group its category belongs to.",
        NoGroup = "Outside groups",
        Template = "Template",
        TemplateHelp = "Every new period gets these incomes and expenses.",
        LoadTemplate = "Load from a JSON file",
        SaveAsTemplate = "Save this period as the template",
        TemplateSaved = "Template saved.",
        InvalidTemplateFile = "This is not a budget template file.",
        LogLevels = new Dictionary<LogSeverity, string>
        {
            [LogSeverity.Verbose] = "Everything",
            [LogSeverity.Debug] = "Debug",
            [LogSeverity.Information] = "Information",
            [LogSeverity.Warning] = "Warnings",
            [LogSeverity.Error] = "Errors",
            [LogSeverity.Fatal] = "Fatal",
        },
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
            ["Import.NothingRead"] = "No receipt lines could be read from the photo.",
            ["Import.NotAnImage"] = "This is not a photo.",
            ["Import.PhotoTooLarge"] = "The photo is larger than 20 MB.",
            ["Import.OcrUnavailable"] = "Reading photos is not available right now.",
            ["Dictionary.NameNotFound"] = "This name is no longer used anywhere.",
            ["Dictionary.SameName"] = "The new name is the same as the old one.",
            ["Dictionary.UnknownKind"] = "There is no such dictionary.",
            ["BudgetName.Empty"] = "Enter a name.",
            ["BudgetName.TooLong"] = "The name can have at most 100 characters.",
            ["FundAmount.Zero"] = "The amount cannot be zero.",
            ["DayOfMonth.OutOfRange"] = "The day must be between 1 and 31.",
            ["BudgetPeriod.EndBeforeStart"] = "The period ends before it starts.",
            ["BudgetPeriod.TooLong"] = "A period can last at most 62 days.",
            ["Budget.PeriodNotFound"] = "There is no such budget period.",
            ["Budget.NoCurrentPeriod"] = "No budget period covers today.",
            ["Budget.FundNotFound"] = "There is no such entry.",
            ["Budget.ItemNotFound"] = "There is no such planned expense.",
            ["Budget.GroupNotFound"] = "There is no such group.",
            ["Budget.GroupNameTaken"] = "A group with this name already exists.",
            ["Budget.GroupInUse"] = "The group still has planned expenses.",
            ["Budget.PeriodOverlaps"] = "This period overlaps another one.",
            ["Http.401"] = "Please sign in again.",
            ["Http.403"] = "You are not allowed to do this.",
            ["Http.404"] = "Not found.",
            ["Http.429"] = "Too many attempts. Please wait a moment.",
        },
    };
}
