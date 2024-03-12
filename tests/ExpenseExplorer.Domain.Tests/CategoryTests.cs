namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Exceptions;
using ExpenseExplorer.Domain.ValueObjects;

public class CategoryTests
{
  [Property(Arbitrary = [typeof(EmptyStringGenerator)])]
  public void ThrowsExceptionWhenNameIsEmpty(string name)
  {
    Action act = () => _ = Category.Create(name);
    act.Should().Throw<EmptyCategoryNameException>();
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator)])]
  public void NameIsTrimmed(string name)
  {
    Category category = Category.Create(name);
    category.Name.Should().Be(name.Trim());
  }
}
