namespace ExpenseExplorer.Application.Tests;

using ExpenseExplorer.Domain.Incomes.Facts;

public class UpdateIncomeDetailsCommandHandlerTests
{
  private const string _originalSource = "item";
  private const decimal _originalAmount = 2;
  private const string _originalCategory = "category";
  private const string _originalDescription = "description";
  private static readonly DateOnly _originalReceivedDate = new(2000, 1, 1);

  private readonly FakeIncomeRepository _incomeRepository =
  [
    Income.Recreate(
        [
          new IncomeCreated("incomeId", _originalSource, _originalAmount, _originalReceivedDate, _originalCategory, _originalDescription, _originalReceivedDate),
        ],
        Version.Create(0UL))
      .Match(_ => throw new UnreachableException(), i => i)
  ];

  [Property(Arbitrary = [typeof(ValidUpdateIncomeDetailsCommandGenerator)])]
  public async Task CanHandleValidCommand(UpdateIncomeDetailsCommand command)
  {
    Income income = await HandleValid(command);
    income.Source.Name.Should().Be(command.Source?.Trim() ?? _originalSource);
    income.Amount.Value.Should().Be(Math.Round(command.Amount ?? _originalAmount, Money.Precision));
    income.Category.Name.Should().Be(command.Category?.Trim() ?? _originalCategory);
    income.ReceivedDate.Date.Should().Be(command.ReceivedDate ?? _originalReceivedDate);
    income.Description.Value.Should().Be(command.Description?.Trim() ?? _originalDescription);
    income.Version.Value.Should().Be(0);
  }

  [Property(Arbitrary = [typeof(ValidUpdateIncomeDetailsCommandGenerator)])]
  public async Task ReturnsFailureWhenReceiptNotFound(UpdateIncomeDetailsCommand command)
  {
    Failure failure = await HandleInvalid(command with { IncomeId = "invalid-Id" });
    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (_, id) => id.Should().Be("invalid-Id"),
      (_, _) => throw new InvalidOperationException("Unexpected validation failure"));
  }

  private async Task<Failure> HandleInvalid(UpdateIncomeDetailsCommand command) => (await Handle(command)).Match(f => f, _ => throw new UnreachableException());

  private async Task<Income> HandleValid(UpdateIncomeDetailsCommand command) => (await Handle(command)).Match(_ => throw new UnreachableException(), i => i);

  private async Task<Result<Income>> Handle(UpdateIncomeDetailsCommand command)
    => await new UpdateIncomeDetailsCommandHandler(_incomeRepository).HandleAsync(command);
}
