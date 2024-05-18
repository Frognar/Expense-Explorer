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

  [Theory]
  [ClassData(typeof(ValidUpdateIncomeDetailsCommandData))]
  public async Task CanHandleValidCommand(UpdateIncomeDetailsCommand command)
  {
    Income income = await HandleValid(command);
    income.Source.Name.Should().Be(command.Source?.Trim() ?? _originalSource);
    income.Amount.Value.Should().Be(command.Amount.HasValue ? Math.Round(command.Amount.Value, Money.Precision) : _originalAmount);
    income.Category.Name.Should().Be(command.Category?.Trim() ?? _originalCategory);
    income.ReceivedDate.Date.Should().Be(command.ReceivedDate ?? _originalReceivedDate);
    income.Description.Value.Should().Be(command.Description?.Trim() ?? _originalDescription);
  }

  [Fact]
  public async Task SavesReceiptWhenValidCommand()
  {
    UpdateIncomeDetailsCommand command = new("incomeId", "s", 1, "c", new DateOnly(2024, 1, 1), "d", new DateOnly(2024, 1, 1));

    _ = await HandleValid(command);

    Income income = _incomeRepository.Single(r => r.Id.Value == "incomeId");
    income.Version.Value.Should().Be(5UL);
  }

  [Property(Arbitrary = [typeof(ValidUpdateIncomeDetailsCommandGenerator)])]
  public async Task ReturnsFailureWhenIncomeNotFound(UpdateIncomeDetailsCommand command)
  {
    Failure failure = await HandleInvalid(command with { IncomeId = "invalid-Id" });
    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (_, id) => id.Should().Be("invalid-Id"),
      (_, _) => throw new InvalidOperationException("Unexpected validation failure"));
  }

  [Property(Arbitrary = [typeof(InvalidUpdateIncomeDetailsCommandGenerator)])]
  public async Task ReturnsFailureWhenInvalidCommand(UpdateIncomeDetailsCommand command)
  {
    Failure failure = await HandleInvalid(command);
    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (_, _) => throw new InvalidOperationException("Unexpected not found failure"),
      (_, errors) => errors.Should().NotBeEmpty());
  }

  private async Task<Failure> HandleInvalid(UpdateIncomeDetailsCommand command) => (await Handle(command)).Match(f => f, _ => throw new UnreachableException());

  private async Task<Income> HandleValid(UpdateIncomeDetailsCommand command) => (await Handle(command)).Match(_ => throw new UnreachableException(), i => i);

  private async Task<Result<Income>> Handle(UpdateIncomeDetailsCommand command)
    => await new UpdateIncomeDetailsCommandHandler(_incomeRepository).HandleAsync(command);
}
