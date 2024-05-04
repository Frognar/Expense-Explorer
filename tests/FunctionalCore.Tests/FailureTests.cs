namespace FunctionalCore.Tests;

public class FailureTests
{
  [Fact]
  public void CanCreateFatalFailure()
  {
    Exception ex = new TestException("TEST");

    FatalFailure failure = Failure.Fatal(ex);

    AssertFatal(failure, ex);
  }

  [Fact]
  public void CanCreateNotFoundFailure()
  {
    NotFoundFailure failure = Failure.NotFound("Not found", "ID");
    AssertNotFound(failure, "Not found", "ID");
  }

  [Fact]
  public void CanCreateValidationFailure()
  {
    ValidationError error = new("ID", "Invalid");

    ValidationFailure failure = Failure.Validation([error]);

    AssertValidationFailure(failure, error);
  }

  [Fact]
  public void CanCreateValidationFailureWithSingleError()
  {
    ValidationError error = new("ID", "Invalid");

    ValidationFailure failure = Failure.Validation(error);

    AssertValidationFailure(failure, error);
  }

  [Fact]
  public void CanCreateValidationFailureWithSinglePropertyError()
  {
    ValidationFailure failure = Failure.Validation("ID", "Invalid");
    AssertValidationFailure(failure, ValidationError.Create("ID", "Invalid"));
  }

  [Fact]
  public void CanRecreateFailure()
  {
    Exception ex = new TestException("TEST");

    FatalFailure failure = Failure.Fatal(ex) with { Message = "Override" };

    failure.Message.Should().Be("Override");
  }

  private static void AssertFatal(FatalFailure failure, Exception ex)
  {
    failure.Exception.Should().Be(ex);
    failure.Message.Should().Be(ex.Message);
  }

  private static void AssertNotFound(NotFoundFailure failure, string message, string id)
  {
    failure.Id.Should().Be(id);
    failure.Message.Should().Be(message);
  }

  private static void AssertValidationFailure(ValidationFailure failure, ValidationError containedError)
  {
    failure.Errors.Should().Contain(containedError);
    failure.Errors.Should().HaveCount(1);
    failure.Message.Should().Be("One or more validation errors occurred.");
  }
}
