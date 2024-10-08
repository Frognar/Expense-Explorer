using DotMaybe;
using DotResult;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Validations;

namespace ExpenseExplorer.Application.Incomes.Commands;

internal static class UpdateIncomeDetailsValidator
{
  public static Result<IncomePatchModel> Validate(UpdateIncomeDetailsCommand command)
  {
    ArgumentNullException.ThrowIfNull(command);
    Func<
      Maybe<Source>,
      Maybe<Money>,
      Maybe<Category>,
      Maybe<NonFutureDate>,
      Maybe<Description>,
      DateOnly,
      IncomePatchModel> createPatchModel = IncomePatchModel.Create;

    return createPatchModel
      .Apply(ValidateSource(command.Source))
      .Apply(ValidateAmount(command.Amount))
      .Apply(ValidateCategory(command.Category))
      .Apply(ValidateReceivedDate(command.ReceivedDate, command.Today))
      .Apply(ValidateDescription(command.Description))
      .Apply(Validation.Succeeded(command.Today))
      .ToResult();
  }

  private static Validated<Maybe<Source>> ValidateSource(string? source)
  {
    if (source is null)
    {
      return Validation.Succeeded(None.OfType<Source>());
    }

    return Source.TryCreate(source)
      .Match(
        () => Validation.Failed<Maybe<Source>>(CommonFailures.EmptySource),
        s => Validation.Succeeded(Some.With(s)));
  }

  private static Validated<Maybe<Money>> ValidateAmount(decimal? amount)
  {
    if (!amount.HasValue)
    {
      return Validation.Succeeded(None.OfType<Money>());
    }

    return Money.TryCreate(amount.Value)
      .Match(
        () => Validation.Failed<Maybe<Money>>(CommonFailures.NegativeUnitPrice),
        a => Validation.Succeeded(Some.With(a)));
  }

  private static Validated<Maybe<Category>> ValidateCategory(string? category)
  {
    if (category is null)
    {
      return Validation.Succeeded(None.OfType<Category>());
    }

    return Category.TryCreate(category)
      .Match(
        () => Validation.Failed<Maybe<Category>>(CommonFailures.EmptyCategory),
        c => Validation.Succeeded(Some.With(c)));
  }

  private static Validated<Maybe<NonFutureDate>> ValidateReceivedDate(DateOnly? date, DateOnly today)
  {
    if (!date.HasValue)
    {
      return Validation.Succeeded(None.OfType<NonFutureDate>());
    }

    return NonFutureDate.TryCreate(date.Value, today)
      .Match(
        () => Validation.Failed<Maybe<NonFutureDate>>(CommonFailures.FuturePurchaseDate),
        receivedDate => Validation.Succeeded(Some.With(receivedDate)));
  }

  private static Validated<Maybe<Description>> ValidateDescription(string? description)
  {
    if (description is null)
    {
      return Validation.Succeeded(None.OfType<Description>());
    }

    return Description.TryCreate(description)
      .Match(
        () => Validation.Failed<Maybe<Description>>(CommonFailures.InvalidDescription),
        d => Validation.Succeeded(Some.With(d)));
  }
}
