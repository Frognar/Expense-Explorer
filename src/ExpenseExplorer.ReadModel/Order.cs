namespace ExpenseExplorer.ReadModel;

using System.Linq.Expressions;

public static class Order
{
  public static OrderingDescriptor<T> AscendingBy<T>(Expression<Func<T, object>> selector)
    => OrderingDescriptor<T>.AscendingBy(selector);

  public static OrderingDescriptor<T> DescendingBy<T>(Expression<Func<T, object>> selector)
    => OrderingDescriptor<T>.DescendingBy(selector);
}
