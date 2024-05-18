namespace ExpenseExplorer.Application.Incomes.Commands;

using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;

internal readonly record struct IncomePatchModel
{
  private IncomePatchModel(
    Maybe<Source> source,
    Maybe<Money> amount,
    Maybe<Category> category,
    Maybe<NonFutureDate> receivedDate,
    Maybe<Description> description)
  {
    Source = source;
    Amount = amount;
    Category = category;
    ReceivedDate = receivedDate;
    Description = description;
  }

  public Maybe<Source> Source { get; }

  public Maybe<Money> Amount { get; }

  public Maybe<Category> Category { get; }

  public Maybe<NonFutureDate> ReceivedDate { get; }

  public Maybe<Description> Description { get; }

  public static IncomePatchModel Create(
    Maybe<Source> source,
    Maybe<Money> amount,
    Maybe<Category> category,
    Maybe<NonFutureDate> receivedDate,
    Maybe<Description> description)
    => new(source, amount, category, receivedDate, description);
}
