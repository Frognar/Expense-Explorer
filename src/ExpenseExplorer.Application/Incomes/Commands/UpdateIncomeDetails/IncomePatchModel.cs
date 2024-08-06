namespace ExpenseExplorer.Application.Incomes.Commands;

using DotMaybe;
using ExpenseExplorer.Domain.ValueObjects;

internal readonly record struct IncomePatchModel
{
  private IncomePatchModel(
    Maybe<Source> source,
    Maybe<Money> amount,
    Maybe<Category> category,
    Maybe<NonFutureDate> receivedDate,
    Maybe<Description> description,
    DateOnly today)
  {
    Source = source;
    Amount = amount;
    Category = category;
    ReceivedDate = receivedDate;
    Description = description;
    Today = today;
  }

  public Maybe<Source> Source { get; }

  public Maybe<Money> Amount { get; }

  public Maybe<Category> Category { get; }

  public Maybe<NonFutureDate> ReceivedDate { get; }

  public Maybe<Description> Description { get; }

  public DateOnly Today { get; }

  public static IncomePatchModel Create(
    Maybe<Source> source,
    Maybe<Money> amount,
    Maybe<Category> category,
    Maybe<NonFutureDate> receivedDate,
    Maybe<Description> description,
    DateOnly today)
    => new(source, amount, category, receivedDate, description, today);
}
