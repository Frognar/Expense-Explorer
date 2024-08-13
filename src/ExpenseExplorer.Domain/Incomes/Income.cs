using DotResult;
using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.Incomes.Facts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore;
using FunctionalCore.Failures;
using Version = ExpenseExplorer.Domain.ValueObjects.Version;

namespace ExpenseExplorer.Domain.Incomes;

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

  public Source Source { get; init; }

  public Money Amount { get; init; }

  public Category Category { get; init; }

  public NonFutureDate ReceivedDate { get; init; }

  public Description Description { get; init; }

  public IEnumerable<Fact> UnsavedChanges { get; private init; }

  public Version Version { get; private init; }

  public static Income New(Source source, Money amount, Category category, NonFutureDate receivedDate, Description description, DateOnly createdDate)
  {
    Id id = Id.Unique();
    IncomeCreated incomeCreated = IncomeCreated.Create(id, source, amount, receivedDate, category, description, createdDate);
    return new Income(id, source, amount, category, receivedDate, description, [incomeCreated], Version.New());
  }

  public static Result<Income> Recreate(IEnumerable<Fact> facts, Version version)
  {
    facts = facts.ToList();
    if (facts.FirstOrDefault() is IncomeCreated incomeCreated)
    {
      return facts.Skip(1)
        .Aggregate(Apply(incomeCreated), (income, fact) => income.Bind(i => i.ApplyFact(fact)))
        .Map(i => i with { Version = version });
    }

    return Fail.OfType<Income>(FailureFactory.Fatal(new ArgumentException("First fact must be an IncomeCreated fact.", nameof(facts))));
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

  public Income CorrectReceivedDate(NonFutureDate newReceivedDate, DateOnly requestedDate)
  {
    Fact fact = ReceivedDateCorrected.Create(Id, newReceivedDate, requestedDate);
    return this with { ReceivedDate = newReceivedDate, UnsavedChanges = UnsavedChanges.Append(fact).ToList() };
  }

  public Income CorrectDescription(Description newDescription)
  {
    Fact fact = DescriptionCorrected.Create(Id, newDescription);
    return this with { Description = newDescription, UnsavedChanges = UnsavedChanges.Append(fact).ToList() };
  }

  public Income Delete()
  {
    return this with { UnsavedChanges = UnsavedChanges.Append(IncomeDeleted.Create(Id)).ToList() };
  }

  public Income ClearChanges()
  {
    return this with { UnsavedChanges = [] };
  }

  public Income WithVersion(Version version)
  {
    return this with { Version = version };
  }

  private Result<Income> ApplyFact(Fact fact)
  {
    return fact switch
    {
      SourceCorrected sourceCorrected => Apply(sourceCorrected),
      AmountCorrected amountCorrected => Apply(amountCorrected),
      CategoryCorrected categoryCorrected => Apply(categoryCorrected),
      ReceivedDateCorrected receivedDateCorrected => Apply(receivedDateCorrected),
      DescriptionCorrected descriptionCorrected => Apply(descriptionCorrected),
      IncomeDeleted _ => Fail.OfType<Income>(FailureFactory.NotFound("Income has been deleted.", Id.Value)),
      _ => Fail.OfType<Income>(FailureFactory.Fatal(new ArgumentException($"Failed to apply fact {fact} to income {this}"))),
    };
  }

  private static Result<Income> Apply(IncomeCreated incomeCreated)
  {
    return (
        from id in Id.TryCreate(incomeCreated.IncomeId)
        from source in Source.TryCreate(incomeCreated.Source)
        from amount in Money.TryCreate(incomeCreated.Amount)
        from category in Category.TryCreate(incomeCreated.Category)
        from receivedDate in NonFutureDate.TryCreate(incomeCreated.ReceivedDate, incomeCreated.CreatedDate)
        from description in Description.TryCreate(incomeCreated.Description)
        select new Income(id, source, amount, category, receivedDate, description, [], Version.New()))
      .ToResult(() => FailureFactory.Fatal(new ArgumentException($"Failed to create income from {incomeCreated}")));
  }

  private Result<Income> Apply(SourceCorrected sourceCorrected)
  {
    return (
        from source in Source.TryCreate(sourceCorrected.Source)
        select this with { Source = source })
      .ToResult(() => FailureFactory.Fatal(new AggregateException($"Failed to apply fact {sourceCorrected} to income {this}")));
  }

  private Result<Income> Apply(AmountCorrected amountCorrected)
  {
    return (
        from amount in Money.TryCreate(amountCorrected.Amount)
        select this with { Amount = amount })
      .ToResult(() => FailureFactory.Fatal(new AggregateException($"Failed to apply fact {amountCorrected} to income {this}")));
  }

  private Result<Income> Apply(CategoryCorrected categoryCorrected)
  {
    return (
        from category in Category.TryCreate(categoryCorrected.Category)
        select this with { Category = category })
      .ToResult(() => FailureFactory.Fatal(new AggregateException($"Failed to apply fact {categoryCorrected} to income {this}")));
  }

  private Result<Income> Apply(ReceivedDateCorrected receivedDateCorrected)
  {
    return (
        from receivedDate in NonFutureDate.TryCreate(receivedDateCorrected.ReceivedDate, receivedDateCorrected.RequestedDate)
        select this with { ReceivedDate = receivedDate })
      .ToResult(() => FailureFactory.Fatal(new AggregateException($"Failed to apply fact {receivedDateCorrected} to income {this}")));
  }

  private Result<Income> Apply(DescriptionCorrected descriptionCorrected)
  {
    return (
        from description in Description.TryCreate(descriptionCorrected.Description)
        select this with { Description = description })
      .ToResult(() => FailureFactory.Fatal(new AggregateException($"Failed to apply fact {descriptionCorrected} to income {this}")));
  }
}
