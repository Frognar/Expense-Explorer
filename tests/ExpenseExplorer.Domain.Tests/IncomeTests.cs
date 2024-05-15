namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Incomes;
using ExpenseExplorer.Domain.Incomes.Facts;

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
}
