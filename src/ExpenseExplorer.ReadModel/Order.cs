namespace ExpenseExplorer.ReadModel;

using System.Linq.Expressions;

public static class Order
{
  public enum Direction
  {
    Ascending,
    Descending,
  }

  public static Direction GetDirection(string sortOrder)
    => sortOrder switch
    {
      "desc" => Direction.Descending,
      _ => Direction.Ascending,
    };

  public static OrderingDescriptor<T> By<T>(Expression<Func<T, object>> selector, Direction direction)
    => direction switch
    {
      Direction.Descending => DescendingBy(selector),
      _ => AscendingBy(selector),
    };

  public static OrderingDescriptor<T> AscendingBy<T>(Expression<Func<T, object>> selector)
    => OrderingDescriptor<T>.AscendingBy(selector);

  public static OrderingDescriptor<T> DescendingBy<T>(Expression<Func<T, object>> selector)
    => OrderingDescriptor<T>.DescendingBy(selector);
}
