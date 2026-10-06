namespace ExpenseExplorer.Contracts.Budget;

// Requests. Every field is nullable so missing values come back as validation errors, not as defaults.

/// <summary>Without a start the period follows the last one; without an end it lasts a month.</summary>
public sealed record CreatePeriodRequest(DateOnly? Start, DateOnly? End);

/// <summary>An income or a change to the money of a period; negative for savings or a smaller income.</summary>
public sealed record FundRequest(string? Name, decimal? Amount);

public sealed record PlanItemRequest(Guid? GroupId, string? Name, decimal? Amount);

public sealed record GroupRequest(string? Name, int? Position);

/// <summary>Puts a receipt category into a group; no group takes it out of every group.</summary>
public sealed record CategoryGroupRequest(string? Category, Guid? GroupId);

/// <summary>The whole template at once. Groups are named; missing ones are created.</summary>
public sealed record TemplateRequest(IReadOnlyList<FundRequest>? Funds, IReadOnlyList<TemplateItemRequest>? Items);

public sealed record TemplateItemRequest(string? Group, string? Name, decimal? Amount);

// Responses.

/// <summary>The id of something just added.</summary>
public sealed record CreatedResponse(Guid Id);

public sealed record PeriodResponse(Guid Id, DateOnly Start, DateOnly End);

public sealed record FundResponse(Guid Id, string Name, decimal Amount);

public sealed record PlanItemResponse(Guid Id, Guid GroupId, string Name, decimal Amount);

public sealed record GroupResponse(Guid Id, string Name, int Position, IReadOnlyList<string> Categories);

/// <summary><see cref="Remaining"/> is negative once spending goes over the plan.</summary>
public sealed record GroupBudgetResponse(
    Guid Id,
    string Name,
    decimal Planned,
    decimal Spent,
    decimal Remaining,
    IReadOnlyList<string> Categories,
    IReadOnlyList<PlanItemResponse> Items);

public sealed record CategorySpentResponse(string Category, decimal Spent);

/// <summary>
/// One period: its money, the groups with plan and spending from receipts, and spending in categories
/// that belong to no group. <see cref="FreePool"/> is roughly what will be left at the end.
/// </summary>
public sealed record BudgetResponse(
    PeriodResponse Period,
    IReadOnlyList<FundResponse> Funds,
    IReadOnlyList<GroupBudgetResponse> Groups,
    IReadOnlyList<CategorySpentResponse> Unassigned,
    decimal TotalFunds,
    decimal Planned,
    decimal Spent,
    decimal FreePool,
    int DaysLeft,
    decimal? PerDay);

/// <summary>
/// One period in short, for comparing periods: plan and spending per group, spending outside groups,
/// and <see cref="FreePool"/>, roughly what is left at the end.
/// </summary>
public sealed record PeriodResultResponse(
    PeriodResponse Period,
    decimal TotalFunds,
    decimal Planned,
    decimal Spent,
    decimal FreePool,
    IReadOnlyList<GroupResultResponse> Groups,
    decimal OutsideGroups);

public sealed record GroupResultResponse(Guid Id, string Name, decimal Planned, decimal Spent);

public sealed record TemplateResponse(IReadOnlyList<FundResponse> Funds, IReadOnlyList<TemplateItemResponse> Items);

public sealed record TemplateItemResponse(Guid Id, string Group, string Name, decimal Amount);
