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
    income.Version.Value.Should().Be(ulong.MaxValue);
  }

  private async Task<Income> HandleValid(AddIncomeCommand command) => (await Handle(command)).Match(_ => throw new UnreachableException(), r => r);

  private async Task<Result<Income>> Handle(AddIncomeCommand command) => await new AddIncomeCommandHandler(_incomeRepository).HandleAsync(command);
}
