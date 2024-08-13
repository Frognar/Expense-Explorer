using System.Diagnostics.CodeAnalysis;

namespace CommandHub.Queries;

[SuppressMessage(
  "Design",
  "CA1040:Avoid empty interfaces",
  Justification = "Marker interface for query pattern.")]
[SuppressMessage(
  "Major Code Smell",
  "S2326:Unused type parameters should be removed",
  Justification = "Marker interface for query pattern.")]
public interface IQuery<TResponse>;
