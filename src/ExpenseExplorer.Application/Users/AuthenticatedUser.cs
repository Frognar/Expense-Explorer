using ExpenseExplorer.Domain.Users;

namespace ExpenseExplorer.Application.Users;

public sealed record AuthenticatedUser(Guid Id, string UserName, UserRole Role);
