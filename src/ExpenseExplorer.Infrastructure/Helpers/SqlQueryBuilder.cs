namespace ExpenseExplorer.Infrastructure.Helpers;

internal static class SqlQueryBuilder
{
    public static string BuildSelectQuery(
        string columnName,
        string tableName,
        IEnumerable<string> searchTerms) =>
        searchTerms.Any()
            ? $"select {columnName} from {tableName} where {BuildWhereClause(searchTerms, columnName)}"
            : $"select {columnName} from {tableName}";

    private static string BuildWhereClause(IEnumerable<string> searchTerms, string columnName) =>
        string.Join(" AND ", CreateLikeConditions(columnName, searchTerms));

    private static IEnumerable<string> CreateLikeConditions(string columnName, IEnumerable<string> searchTerms) =>
        searchTerms.Select(term => CreateLikeCondition(columnName, term));

    private static string CreateLikeCondition(string columnName, string searchTerm) =>
        $"UPPER({columnName}) LIKE '%{searchTerm.ToUpperInvariant()}%'";
}