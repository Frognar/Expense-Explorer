namespace ExpenseExplorer.Domain.Tests;

public class CategoryTests
{
  [Property(Arbitrary = [typeof(EmptyOrWhiteSpaceStringGenerator)])]
  public void ThrowsExceptionWhenNameIsEmpty(string name)
  {
    Action act = () => _ = Category.Create(name);
    act.Should().Throw<EmptyCategoryNameException>();
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator)])]
  public void NameIsTrimmed(string name)
  {
    Category.Create(name).Name.Should().Be(name.Trim());
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator)])]
  public void NameIsTrimmedWithRecordSyntax(string name)
  {
    Category category = Category.Create("123") with { Name = name };
    category.Name.Should().Be(name.Trim());
  }

  [Property(Arbitrary = [typeof(EmptyOrWhiteSpaceStringGenerator)])]
  public void ThrowsExceptionWhenNameIsEmptyWithRecordSyntax(string name)
  {
    Action act = () => _ = Category.Create("123") with { Name = name };
    act.Should().Throw<EmptyCategoryNameException>();
  }
}
