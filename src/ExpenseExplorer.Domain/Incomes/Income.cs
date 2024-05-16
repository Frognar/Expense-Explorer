namespace ExpenseExplorer.Domain.Incomes;

using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.Incomes.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public sealed record Income
{
  private Income(
    Id id,
    Source source,
    Money amount,
    Category category,
    NonFutureDate receivedDate,
    Description description,
    IEnumerable<Fact> unsavedChanges,
    Version version)
  {
    Id = id;
    Source = source;
    Amount = amount;
    Category = category;
    ReceivedDate = receivedDate;
    Description = description;
    UnsavedChanges = unsavedChanges;
    Version = version;
  }

  public Id Id { get; }

  public Source Source { get; private init; }

  public Money Amount { get; private init; }

  public Category Category { get; private init; }

  public NonFutureDate ReceivedDate { get; private init; }

  public Description Description { get; private init; }

  public IEnumerable<Fact> UnsavedChanges { get; private init; }

  public Version Version { get; private init; }

  public static Income New(Source source, Money amount, Category category, NonFutureDate receivedDate, Description description, DateOnly createdDate)
  {
    Id id = Id.Unique();
    IncomeCreated incomeCreated = IncomeCreated.Create(id, source, amount, receivedDate, category, description, createdDate);
    return new Income(id, source, amount, category, receivedDate, description, [incomeCreated], Version.New());
  }

  public Income CorrectSource(Source newSource)
  {
    Fact fact = SourceCorrected.Create(Id, newSource);
    return this with { Source = newSource, UnsavedChanges = UnsavedChanges.Append(fact).ToList() };
  }

  public Income ClearChanges()
  {
    return this with { UnsavedChanges = [] };
  }

  public Income WithVersion(Version version)
  {
    return this with { Version = version };
  }
}
