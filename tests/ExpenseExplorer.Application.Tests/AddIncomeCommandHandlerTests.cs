namespace ExpenseExplorer.Application.Tests;

public class AddIncomeCommandHandlerTests
{
  private readonly FakeIncomeRepository _incomeRepository = new();

  [Property(Arbitrary = [typeof(ValidAddIncomeCommandGenerator)])]
  public async Task CanHandleValidCommand(AddIncomeCommand command)
  {
    Income income = await HandleValid(command);
    income.Source.Name.Should().Be(command.Source.Trim());
    income.Amount.Value.Should().Be(Math.Round(command.Amount, Money.Precision));
    income.Category.Name.Should().Be(command.Category.Trim());
    income.ReceivedDate.Date.Should().Be(command.ReceivedDate);
    income.Description.Value.Should().Be(command.Description?.Trim() ?? string.Empty);
    income.Version.Value.Should().Be(0);
  }

  [Property(Arbitrary = [typeof(InvalidAddIncomeCommandGenerator)])]
  public async Task CanHandleInvalidCommand(AddIncomeCommand command)
  {
    Failure failure = await HandleInvalid(command);
    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (_, _) => throw new InvalidOperationException("Unexpected not found failure"),
      (_, errors) => errors.Should().NotBeEmpty());
  }

  [Property(Arbitrary = [typeof(ValidAddIncomeCommandGenerator)])]
  public async Task SavesReceiptWhenValidCommand(AddIncomeCommand command)
  {
    Income income = await HandleValid(command);
    _incomeRepository.Should().Contain(r => r.Id == income.Id);
  }

  private async Task<Failure> HandleInvalid(AddIncomeCommand command) => (await Handle(command)).Match(f => f, _ => throw new UnreachableException());

  private async Task<Income> HandleValid(AddIncomeCommand command) => (await Handle(command)).Match(_ => throw new UnreachableException(), i => i);

  private async Task<Result<Income>> Handle(AddIncomeCommand command) => await new AddIncomeCommandHandler(_incomeRepository).HandleAsync(command);
}
