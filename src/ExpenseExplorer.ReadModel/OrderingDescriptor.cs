using System.Linq.Expressions;

namespace ExpenseExplorer.ReadModel;

public readonly record struct OrderingDescriptor<T>(Expression<Func<T, object>> Selector, bool IsDescending = false)
{
  internal static OrderingDescriptor<T> AscendingBy(Expression<Func<T, object>> selector) => new(selector);

  internal static OrderingDescriptor<T> DescendingBy(Expression<Func<T, object>> selector) => new(selector, true);

  internal IOrderedQueryable<T> Apply(IQueryable<T> source)
  {
    ArgumentNullException.ThrowIfNull(source);
    return IsDescending ? source.OrderByDescending(Selector) : source.OrderBy(Selector);
  }

  internal IOrderedQueryable<T> Apply(IOrderedQueryable<T> source)
  {
    ArgumentNullException.ThrowIfNull(source);
    return IsDescending ? source.ThenByDescending(Selector) : source.ThenBy(Selector);
  }
}
