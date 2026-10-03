using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Application.Budget;

public static class BudgetErrors
{
    public static readonly Error PeriodNotFound = new("Budget.PeriodNotFound", "There is no such budget period.", ErrorType.NotFound);

    public static readonly Error NoCurrentPeriod = new("Budget.NoCurrentPeriod", "No budget period covers today.", ErrorType.NotFound);

    public static readonly Error FundNotFound = new("Budget.FundNotFound", "There is no such income or change.", ErrorType.NotFound);

    public static readonly Error ItemNotFound = new("Budget.ItemNotFound", "There is no such planned expense.", ErrorType.NotFound);

    public static readonly Error GroupNotFound = new("Budget.GroupNotFound", "There is no such budget group.", ErrorType.NotFound);

    public static readonly Error UnknownGroup = new("Budget.GroupNotFound", "There is no such budget group.", Target: "groupId");

    public static readonly Error GroupNameTaken = new("Budget.GroupNameTaken", "A group with this name already exists.", ErrorType.Conflict, "name");

    public static readonly Error GroupInUse = new("Budget.GroupInUse", "Planned expenses still belong to this group.", ErrorType.Conflict);

    public static readonly Error PeriodOverlaps = new("Budget.PeriodOverlaps", "The period overlaps another one.", ErrorType.Conflict, "start");

    public static readonly Error StartRequired = new("Input.Required", "Value is required.", Target: "start");
}
