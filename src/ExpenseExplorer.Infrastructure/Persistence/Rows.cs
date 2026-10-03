namespace ExpenseExplorer.Infrastructure.Persistence;

// Storage shape of a receipt. Only the repository and read queries touch these;
// everything else works with the domain model.

internal sealed class ReceiptRow
{
    public Guid Id { get; set; }

    public string Store { get; set; } = "";

    public DateOnly PurchaseDate { get; set; }

    public List<ReceiptItemRow> Items { get; } = [];
}

internal sealed class ReceiptItemRow
{
    public Guid Id { get; set; }

    public Guid ReceiptId { get; set; }

    public int Position { get; set; }

    public string Item { get; set; } = "";

    public string Category { get; set; } = "";

    public decimal Quantity { get; set; }

    public decimal Amount { get; set; }

    public decimal Discount { get; set; }

    public string? Description { get; set; }
}

/// <summary>An old name kept after a rename, so imports that still read it get the new one.</summary>
internal sealed class NameAliasRow
{
    /// <summary>"store", "item" or "category".</summary>
    public string Kind { get; set; } = "";

    /// <summary>The old name in upper case, so letter case does not matter.</summary>
    public string Alias { get; set; } = "";

    public string Name { get; set; } = "";
}

// Budget. Incomes and planned expenses without a period make up the template for new periods.

internal sealed class BudgetGroupRow
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public int Position { get; set; }
}

/// <summary>A receipt category counted in a budget group; a category belongs to at most one group.</summary>
internal sealed class BudgetCategoryRow
{
    public string Category { get; set; } = "";

    public Guid GroupId { get; set; }
}

internal sealed class BudgetPeriodRow
{
    public Guid Id { get; set; }

    public DateOnly Start { get; set; }

    public DateOnly End { get; set; }
}

internal sealed class BudgetFundRow
{
    public Guid Id { get; set; }

    /// <summary><c>null</c> for the template.</summary>
    public Guid? PeriodId { get; set; }

    public int Position { get; set; }

    public string Name { get; set; } = "";

    public decimal Amount { get; set; }
}

internal sealed class BudgetItemRow
{
    public Guid Id { get; set; }

    /// <summary><c>null</c> for the template.</summary>
    public Guid? PeriodId { get; set; }

    public Guid GroupId { get; set; }

    public int Position { get; set; }

    public string Name { get; set; } = "";

    public decimal Amount { get; set; }
}
