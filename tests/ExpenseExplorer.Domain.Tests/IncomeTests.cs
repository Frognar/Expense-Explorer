namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Incomes;
using ExpenseExplorer.Domain.Incomes.Facts;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public class IncomeTests
{
  [Property(
    Arbitrary =
    [
      typeof(SourceGenerator),
      typeof(MoneyGenerator),
      typeof(NonFutureDateGenerator),
      typeof(CategoryGenerator),
      typeof(DescriptionGenerator),
    ])]
  public void CanBeCreated(Source source, Money amount, NonFutureDate receivedDate, Category category, Description description)
  {
    Income receipt = Income.New(source, amount, category, receivedDate, description, receivedDate.Date);
    receipt.Should().NotBeNull();
    receipt.Id.Should().NotBeNull();
    receipt.Source.Should().Be(source);
    receipt.Amount.Should().Be(amount);
    receipt.ReceivedDate.Should().Be(receivedDate);
    receipt.Category.Should().Be(category);
    receipt.Description.Should().Be(description);
    receipt.UnsavedChanges.Should().HaveCount(1);
    receipt.Version.Should().Be(Version.Create(ulong.MaxValue));
  }

  [Property(
    Arbitrary =
    [
      typeof(SourceGenerator),
      typeof(MoneyGenerator),
      typeof(NonFutureDateGenerator),
      typeof(CategoryGenerator),
      typeof(DescriptionGenerator),
    ])]
  public void ProducesIncomeCreatedFactWhenCreated(Source source, Money amount, NonFutureDate receivedDate, Category category, Description description)
  {
    Income income = Income.New(source, amount, category, receivedDate, description, receivedDate.Date);
    income.UnsavedChanges.Count().Should().Be(1);
    IncomeCreated incomeCreated = income.UnsavedChanges.OfType<IncomeCreated>().Single();
    incomeCreated.IncomeId.Should().Be(income.Id.Value);
    incomeCreated.Source.Should().Be(source.Name);
    incomeCreated.Amount.Should().Be(amount.Value);
    incomeCreated.ReceivedDate.Should().Be(receivedDate.Date);
    incomeCreated.Category.Should().Be(category.Name);
    incomeCreated.Description.Should().Be(description.Value);
    incomeCreated.CreatedDate.Should().Be(receivedDate.Date);
  }

  [Property(Arbitrary = [typeof(IncomeGenerator), typeof(SourceGenerator)])]
  public void CanCorrectSource(Income income, Source newSource)
  {
    income
      .CorrectSource(newSource)
      .Source
      .Should()
      .Be(newSource);
  }

  [Property(Arbitrary = [typeof(IncomeGenerator), typeof(SourceGenerator)])]
  public void ProducesSourceUpdatedFactWhenSourceUpdated(Income income, Source newSource)
  {
    income = income.CorrectSource(newSource);
    SourceCorrected sourceCorrected = income.UnsavedChanges.OfType<SourceCorrected>().Single();
    sourceCorrected.IncomeId.Should().Be(income.Id.Value);
    sourceCorrected.Source.Should().Be(newSource.Name);
  }

  [Property(Arbitrary = [typeof(IncomeGenerator), typeof(MoneyGenerator)])]
  public void CanCorrectAmount(Income income, Money newAmount)
  {
    income
      .CorrectAmount(newAmount)
      .Amount
      .Should()
      .Be(newAmount);
  }

  [Property(Arbitrary = [typeof(IncomeGenerator), typeof(MoneyGenerator)])]
  public void ProducesAmountUpdatedFactWhenAmountUpdated(Income income, Money newAmount)
  {
    income = income.CorrectAmount(newAmount);
    AmountCorrected amountCorrected = income.UnsavedChanges.OfType<AmountCorrected>().Single();
    amountCorrected.IncomeId.Should().Be(income.Id.Value);
    amountCorrected.Amount.Should().Be(newAmount.Value);
  }

  [Property(Arbitrary = [typeof(IncomeGenerator), typeof(CategoryGenerator)])]
  public void CanCorrectCategory(Income income, Category newCategory)
  {
    income
      .CorrectCategory(newCategory)
      .Category
      .Should()
      .Be(newCategory);
  }

  [Property(Arbitrary = [typeof(IncomeGenerator), typeof(CategoryGenerator)])]
  public void ProducesCategoryUpdatedFactWhenCategoryUpdated(Income income, Category newCategory)
  {
    income = income.CorrectCategory(newCategory);
    CategoryCorrected categoryCorrected = income.UnsavedChanges.OfType<CategoryCorrected>().Single();
    categoryCorrected.IncomeId.Should().Be(income.Id.Value);
    categoryCorrected.Category.Should().Be(newCategory.Name);
  }

  [Property(Arbitrary = [typeof(IncomeGenerator), typeof(NonFutureDateGenerator)])]
  public void CanCorrectReceivedDate(Income income, NonFutureDate newReceivedDate)
  {
    income
      .CorrectReceivedDate(newReceivedDate)
      .ReceivedDate
      .Should()
      .Be(newReceivedDate);
  }

  [Property(Arbitrary = [typeof(IncomeGenerator), typeof(NonFutureDateGenerator)])]
  public void ProducesReceivedDateUpdatedFactWhenReceivedDateUpdated(Income income, NonFutureDate newReceivedDate)
  {
    income = income.CorrectReceivedDate(newReceivedDate);
    ReceivedDateCorrected receivedDateCorrected = income.UnsavedChanges.OfType<ReceivedDateCorrected>().Single();
    receivedDateCorrected.IncomeId.Should().Be(income.Id.Value);
    receivedDateCorrected.ReceivedDate.Should().Be(newReceivedDate.Date);
  }

  [Property(Arbitrary = [typeof(IncomeGenerator), typeof(DescriptionGenerator)])]
  public void CanCorrectDescription(Income income, Description newDescription)
  {
    income
      .CorrectDescription(newDescription)
      .Description
      .Should()
      .Be(newDescription);
  }

  [Property(Arbitrary = [typeof(IncomeGenerator), typeof(DescriptionGenerator)])]
  public void ProducesDescriptionUpdatedFactWhenDescriptionUpdated(Income income, Description newDescription)
  {
    income = income.CorrectDescription(newDescription);
    DescriptionCorrected descriptionCorrected = income.UnsavedChanges.OfType<DescriptionCorrected>().Single();
    descriptionCorrected.IncomeId.Should().Be(income.Id.Value);
    descriptionCorrected.Description.Should().Be(newDescription.Value);
  }

  [Fact]
  public void CanBeRecreatedFromFacts()
  {
    DateOnly today = new DateOnly(2000, 1, 1);
    List<Fact> facts =
    [
      new IncomeCreated("id", "s", 0, today, "c", "d", today),
      new SourceCorrected("id", "source"),
      new AmountCorrected("id", 100),
      new CategoryCorrected("id", "category"),
      new ReceivedDateCorrected("id", today.AddDays(-1)),
      new DescriptionCorrected("id", "description")
    ];

    Result<Income> resultOfIncome = Income.Recreate(facts, Version.Create((ulong)(facts.Count - 1)));
    Income income = resultOfIncome.Match(_ => throw new UnreachableException(), i => i);

    income.Id.Value.Should().Be("id");
    income.Source.Name.Should().Be("source");
    income.Amount.Value.Should().Be(100);
    income.Category.Name.Should().Be("category");
    income.ReceivedDate.Date.Should().Be(today.AddDays(-1));
    income.Description.Value.Should().Be("description");
    income.UnsavedChanges.Should().BeEmpty();
    income.Version.Value.Should().Be((ulong)(facts.Count - 1));
  }

  [Fact]
  public void ReturnsFailureWhenRecreatedWithoudIncomeCreatedFact()
  {
    DateOnly today = new DateOnly(2000, 1, 1);
    List<Fact> facts =
    [
      new SourceCorrected("id", "source"),
      new AmountCorrected("id", 100),
      new CategoryCorrected("id", "category"),
      new ReceivedDateCorrected("id", today.AddDays(-1)),
      new DescriptionCorrected("id", "description")
    ];

    Result<Income> resultOfIncome = Income.Recreate(facts, Version.Create((ulong)(facts.Count - 1)));
    Failure failure = resultOfIncome.Match(f => f, _ => throw new UnreachableException());

    failure.Should().NotBeNull();
  }

  [Fact]
  public void ReturnsFailureWhenRecreatedWithUnsupportedFact()
  {
    Fact unknown = new UnknownFact();

    Result<Income> resultOfReceipt = Income.Recreate([unknown], Version.Create(0UL));
    Failure failure = resultOfReceipt.Match(f => f, _ => throw new UnreachableException());

    failure.Should().NotBeNull();
  }

  private sealed record UnknownFact : Fact;
}
