namespace ExpenseExplorer.Domain.Common;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
}

/// <summary>
/// Describes why an operation failed. <see cref="Code"/> is stable and meant for clients,
/// <see cref="Target"/> optionally names the input the error refers to.
/// </summary>
public sealed record Error(string Code, string Message, ErrorType Type = ErrorType.Validation, string? Target = null)
{
    public Error For(string target) => this with { Target = target };
}
