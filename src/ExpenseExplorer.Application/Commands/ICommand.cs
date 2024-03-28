namespace ExpenseExplorer.Application.Commands;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage(
  "Design",
  "CA1040:Avoid empty interfaces",
  Justification = "Marker interface for command pattern.")]
[SuppressMessage(
  "Major Code Smell",
  "S2326:Unused type parameters should be removed",
  Justification = "Marker interface for command pattern.")]
public interface ICommand<TResponse>;
