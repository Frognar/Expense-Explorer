namespace ExpenseExplorer.Domain.Common;

/// <summary>
/// Describes why an operation failed. <see cref="Code"/> is stable and meant for clients,
/// <see cref="Target"/> optionally names the input the error refers to.
/// </summary>
public sealed record Error(string Code, string Message, string? Target = null)
{
    public Error For(string target) => this with { Target = target };
}
