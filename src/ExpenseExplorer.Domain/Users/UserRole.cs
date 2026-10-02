namespace ExpenseExplorer.Domain.Users;

/// <summary>What a signed-in user may do. Every account has exactly one role; each role can do all the ones before it.</summary>
public enum UserRole
{
    /// <summary>Can browse everything but change nothing.</summary>
    Reader,

    /// <summary>Can browse and change everything.</summary>
    Editor,

    /// <summary>Can do what an editor can and also read the application logs.</summary>
    Admin,
}
