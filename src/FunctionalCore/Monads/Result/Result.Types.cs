namespace FunctionalCore.Monads;

using FunctionalCore.Failures;

public readonly partial record struct Result<T>
{
  private interface IResult;

  private readonly record struct FailureType(Failure Failure) : IResult;

  private readonly record struct SuccessType(T Value) : IResult;
}
