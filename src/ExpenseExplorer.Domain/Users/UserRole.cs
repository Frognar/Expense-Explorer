namespace ExpenseExplorer.Domain.Users;

/// <summary>What a signed-in user may do. Every account has exactly one role.</summary>
public enum UserRole
{
    /// <summary>Can browse everything but change nothing.</summary>
    Reader,

    /// <summary>Can browse and change everything.</summary>
    Editor,
}
