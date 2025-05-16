namespace ExpenseExplorer.Application;

public static class Validation
{
    public static Validated<T> Succeed<T>(T value) => Validated<T>.Succeed(value);
    public static Validated<T> Failed<T>(IEnumerable<ValidationError> errors) => Validated<T>.Failed(errors);
}