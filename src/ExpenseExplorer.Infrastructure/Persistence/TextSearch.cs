using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.Infrastructure.Persistence;

/// <summary>
/// One way of searching text everywhere: case and Polish diacritics are ignored ("zolty" finds "Żółty"),
/// and when nothing contains the search, texts that are merely similar are found instead ("biedronak" finds "Biedronka").
/// The comparisons run in Postgres, in functions created by the <c>TextSearch</c> migration.
/// </summary>
internal static class TextSearch
{
    /// <summary>Shorter searches have too few letter triples to tell a typo from a different word.</summary>
    public const int SimilarMinimumLength = 3;

    /// <summary>Lowest pg_trgm word similarity still treated as a match; picked on real item and store names.</summary>
    public const float SimilarMinimumScore = 0.3f;

    /// <summary><c>expense.search_contains</c>: whether <paramref name="text"/> contains <paramref name="search"/>, ignoring case and diacritics.</summary>
    public static bool Contains(string text, string search) =>
        throw new InvalidOperationException("Translated to SQL only.");

    /// <summary><c>expense.search_similarity</c>: from 0 to 1, how closely some part of <paramref name="text"/> resembles <paramref name="search"/>.</summary>
    public static float Similarity(string text, string search) =>
        throw new InvalidOperationException("Translated to SQL only.");

    public static void Register(ModelBuilder builder)
    {
        builder.HasDbFunction(typeof(TextSearch).GetMethod(nameof(Contains))!).HasName("search_contains").HasSchema(ExpenseExplorerDbContext.Schema);
        builder.HasDbFunction(typeof(TextSearch).GetMethod(nameof(Similarity))!).HasName("search_similarity").HasSchema(ExpenseExplorerDbContext.Schema);
    }

    /// <summary>
    /// Decides how <paramref name="search"/> matches <paramref name="texts"/>: by containment when any text contains it,
    /// by similarity otherwise. A blank search matches everything.
    /// </summary>
    public static async Task<TextMatch> MatchAsync(IQueryable<string> texts, string search, CancellationToken cancellationToken)
    {
        if (search.Length == 0)
        {
            return TextMatch.Everything;
        }

        bool anyContains = await texts.AnyAsync(text => Contains(text, search), cancellationToken);
        return anyContains || search.Length < SimilarMinimumLength
            ? new TextMatch(search, TextMatchKind.Contains)
            : new TextMatch(search, TextMatchKind.Similar);
    }
}

internal enum TextMatchKind
{
    Everything,
    Contains,
    Similar,
}

/// <summary>How a search was decided to match, applied to any query by naming the searched text.</summary>
internal sealed record TextMatch(string Search, TextMatchKind Kind)
{
    public static readonly TextMatch Everything = new("", TextMatchKind.Everything);

    public IQueryable<T> Filter<T>(IQueryable<T> rows, Expression<Func<T, string>> text) => Kind switch
    {
        TextMatchKind.Contains => rows.Where(Over(text, value => TextSearch.Contains(value, Search))),
        TextMatchKind.Similar => rows.Where(Over(text, value => TextSearch.Similarity(value, Search) >= TextSearch.SimilarMinimumScore)),
        _ => rows,
    };

    /// <summary>Similar matches come most similar first; otherwise the order is just <paramref name="text"/>.</summary>
    public IOrderedQueryable<T> BestFirst<T>(IQueryable<T> rows, Expression<Func<T, string>> text) =>
        Kind == TextMatchKind.Similar
            ? rows.OrderByDescending(Over(text, value => TextSearch.Similarity(value, Search))).ThenBy(text)
            : rows.OrderBy(text);

    /// <summary>Builds <c>row => condition(text(row))</c> as an expression EF can translate.</summary>
    private static Expression<Func<T, TResult>> Over<T, TResult>(Expression<Func<T, string>> text, Expression<Func<string, TResult>> condition) =>
        Expression.Lambda<Func<T, TResult>>(
            new ParameterReplacer(condition.Parameters[0], text.Body).Visit(condition.Body),
            text.Parameters);

    private sealed class ParameterReplacer(ParameterExpression parameter, Expression replacement) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) =>
            node == parameter ? replacement : base.VisitParameter(node);
    }
}
