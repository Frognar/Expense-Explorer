namespace ExpenseExplorer.Domain.Incomes;

using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.Incomes.Facts;
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
    IEnumerable<Fact> unsavedChanges,
    Version version)
  {
    Id = id;
    Source = source;
    Amount = money;
    ReceivedDate = receivedDate;
    Description = description;
    Category = category;
    UnsavedChanges = unsavedChanges;
    Version = version;
  }

  public Id Id { get; }

  public Source Source { get; private init; }

  public Money Amount { get; private init; }

  public NonFutureDate ReceivedDate { get; private init; }

  public Description Description { get; private init; }

  public Category Category { get; private init; }

  public IEnumerable<Fact> UnsavedChanges { get; private init; }

  public Version Version { get; private init; }

  public static Income New(Source source, Money amount, NonFutureDate receivedDate, Category category, Description description, DateOnly createdDate)
  {
    Id id = Id.Unique();
    IncomeCreated incomeCreated = IncomeCreated.Create(id, source, amount, receivedDate, category, description, createdDate);
    return new Income(id, source, amount, receivedDate, description, category, [incomeCreated], Version.New());
  }
}
