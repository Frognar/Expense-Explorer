namespace ExpenseExplorer.ReadModel;

using System.Linq.Expressions;

public static class QueryableExtensions
{
  public static IQueryable<T> GetPage<T>(this IQueryable<T> source, int pageNumber, int pageSize)
  {
    ArgumentNullException.ThrowIfNull(source);
    return source.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
  }

  public static IQueryable<T> OrderByMany<T>(
    this IQueryable<T> source,
    OrderingDescriptor<T> selector,
    params OrderingDescriptor<T>[] selectors)
  {
    ArgumentNullException.ThrowIfNull(source);
    IOrderedQueryable<T> ordered = selector.Apply(source);
    return selectors.Aggregate(ordered, (current, s) => s.Apply(current));
  }

  public static IQueryable<T> Filter<T>(
    this IQueryable<T> source,
    Expression<Func<T, bool>> predicate,
    params Expression<Func<T, bool>>[] predicates)
  {
    ArgumentNullException.ThrowIfNull(source);
    IQueryable<T> filtered = source.Where(predicate);
    return predicates.Aggregate(filtered, (current, p) => current.Where(p));
  }
}
