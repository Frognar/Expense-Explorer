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

  public Income CorrectAmount(Money newAmount)
  {
    Fact fact = AmountCorrected.Create(Id, newAmount);
    return this with { Amount = newAmount, UnsavedChanges = UnsavedChanges.Append(fact).ToList() };
  }

  public Income CorrectCategory(Category newCategory)
  {
    Fact fact = CategoryCorrected.Create(Id, newCategory);
    return this with { Category = newCategory, UnsavedChanges = UnsavedChanges.Append(fact).ToList() };
  }

  public Income CorrectReceivedDate(NonFutureDate newReceivedDate)
  {
    Fact fact = ReceivedDateCorrected.Create(Id, newReceivedDate);
    return this with { ReceivedDate = newReceivedDate, UnsavedChanges = UnsavedChanges.Append(fact).ToList() };
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