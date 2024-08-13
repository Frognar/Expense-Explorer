using ExpenseExplorer.Domain.Incomes.Facts;
using ExpenseExplorer.Tests.Common;
using FunctionalCore;

namespace ExpenseExplorer.Application.Tests;

public class DeleteIncomeCommandHandlerTests
{
  private readonly FakeIncomeRepository _incomeRepository =
  [
    Income.Recreate(
        [new IncomeCreated("incomeId", "item", 2, new DateOnly(2000, 1, 1), "category", "description", new DateOnly(2000, 1, 1))],
        Version.Create(0UL))
      .Match(_ => throw new UnreachableException(), i => i)
  ];

  [Fact]
  public async Task DeletesIncome()
  {
    DeleteIncomeCommand command = new("incomeId");

    Unit result = await HandleValid(command);

    result.Should().Be(Unit.Instance);
  }

  [Fact]
  public async Task SavesIncomeWhenValidCommand()
  {
    DeleteIncomeCommand command = new("incomeId");

    _ = await HandleValid(command);

    Income income = _incomeRepository.Single(i => i.Id.Value == command.IncomeId);
    income.Version.Value.Should().Be(1UL);
  }

  [Fact]
  public async Task ReturnsFailureWhenIncomeNotFound()
  {
    DeleteIncomeCommand command = new("invalid-Id");

    Failure failure = await HandleInvalid(command);

    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (_, id) => id.Should().Be("invalid-Id"),
      (_, _) => throw new InvalidOperationException("Unexpected validation failure"));
  }

  private async Task<Failure> HandleInvalid(DeleteIncomeCommand command) => (await Handle(command)).Match(f => f, _ => throw new UnreachableException());

  private async Task<Unit> HandleValid(DeleteIncomeCommand command) => (await Handle(command)).Match(_ => throw new UnreachableException(), u => u);

  private async Task<Result<Unit>> Handle(DeleteIncomeCommand command) => await new DeleteIncomeCommandHandler(_incomeRepository).HandleAsync(command);
}
