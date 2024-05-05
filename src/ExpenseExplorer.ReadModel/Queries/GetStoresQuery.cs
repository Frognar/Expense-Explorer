namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;
using FunctionalCore.Monads;

public sealed record GetStoresQuery(
  int PageSize,
  int PageNumber,
  string Search)
  : IQuery<Result<PageOf<Store>>>;
