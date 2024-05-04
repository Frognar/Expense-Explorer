namespace FunctionalCore.Tests;

public class FailureTests
{
  [Fact]
  public void CanCreateFatalFailure()
  {
    Exception ex = new TestException("TEST");

    Failure failure = Failure.Fatal(ex);

    failure.Match(
      (message, exception) => AssertFatal(message, exception, ex),
      (_, _) => throw new InvalidOperationException("Unexpected not found failure"),
      (_, _) => throw new InvalidOperationException("Unexpected validation failure"));
  }

  [Fact]
  public void CanCreateNotFoundFailure()
  {
    Failure failure = Failure.NotFound("Not found", "ID");

    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (message, id) => AssertNotFound(message, id, "Not found", "ID"),
      (_, _) => throw new InvalidOperationException("Unexpected validation failure"));
  }

  [Fact]
  public void CanCreateValidationFailure()
  {
    ValidationError error = new("ID", "Invalid");

    Failure failure = Failure.Validation([error]);

    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (_, _) => throw new InvalidOperationException("Unexpected not found failure"),
      (message, errors) => AssertValidationFailure(message, errors, error));
  }

  [Fact]
  public void CanCreateValidationFailureWithSingleError()
  {
    ValidationError error = new("ID", "Invalid");

    Failure failure = Failure.Validation(error);

    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (_, _) => throw new InvalidOperationException("Unexpected not found failure"),
      (message, errors) => AssertValidationFailure(message, errors, error));
  }

  [Fact]
  public void CanCreateValidationFailureWithSinglePropertyError()
  {
    Failure failure = Failure.Validation("ID", "Invalid");

    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (_, _) => throw new InvalidOperationException("Unexpected not found failure"),
      (message, errors) => AssertValidationFailure(message, errors, ValidationError.Create("ID", "Invalid")));
  }

  private static Unit AssertFatal(string failureMessage, Exception failureException, Exception ex)
  {
    failureMessage.Should().Be(ex.Message);
    failureException.Should().Be(ex);
    return Unit.Instance;
  }

  private static Unit AssertNotFound(string failureMessage, string failureId, string message, string id)
  {
    failureMessage.Should().Be(message);
    failureId.Should().Be(id);
    return Unit.Instance;
  }

  private static Unit AssertValidationFailure(
    string failureMessage,
    IEnumerable<ValidationError> failureErrors,
    ValidationError containedError)
  {
    failureMessage.Should().Be("One or more validation errors occurred.");
    failureErrors.Should().BeEquivalentTo([containedError]);
    return Unit.Instance;
  }
}
