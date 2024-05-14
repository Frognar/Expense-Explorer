namespace ExpenseExplorer.Domain.Incomes;

using ExpenseExplorer.Domain.ValueObjects;

public sealed record Income
{
  private Income(
    Id id,
    Source source,
    Money money,
    NonFutureDate receivedDate,
    Description description,
    Category category,
    Version version)
  {
    Id = id;
    Source = source;
    Amount = money;
    ReceivedDate = receivedDate;
    Description = description;
    Category = category;
    Version = version;
  }

  public Id Id { get; }

  public Source Source { get; private init; }

  public Money Amount { get; private init; }

  public NonFutureDate ReceivedDate { get; private init; }

  public Description Description { get; private init; }

  public Category Category { get; private init; }

  public Version Version { get; private init; }

  public static Income New(Source source, Money amount, NonFutureDate receivedDate, Category category, Description description, DateOnly createdDate)
  {
    return new Income(Id.Unique(), source, amount, receivedDate, description, category, Version.New());
  }
}
