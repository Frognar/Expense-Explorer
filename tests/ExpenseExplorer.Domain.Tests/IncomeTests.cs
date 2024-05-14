namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Incomes;

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
    Income receipt = Income.New(source, amount, receivedDate, category, description, receivedDate.Date);
    receipt.Should().NotBeNull();
    receipt.Id.Should().NotBeNull();
    receipt.Source.Should().Be(source);
    receipt.Amount.Should().Be(amount);
    receipt.ReceivedDate.Should().Be(receivedDate);
    receipt.Category.Should().Be(category);
    receipt.Description.Should().Be(description);
    receipt.Version.Should().Be(Version.Create(ulong.MaxValue));
  }
}
