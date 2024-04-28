namespace FunctionalCore.Tests;

using FunctionalCore.Failures;

public class FailureTests
{
  [Fact]
  public void CanCreateFatalFailure()
  {
    Exception ex = new TestException("TEST");

    FatalFailure failure = new FatalFailure(ex);

    failure.Exception.Should().Be(ex);
    failure.Message.Should().Be(ex.Message);
  }

  [Fact]
  public void CanRecreateFatalFailure()
  {
    Exception ex = new TestException("TEST");

    FatalFailure failure = new FatalFailure(new FatalFailure(ex));

    failure.Exception.Should().Be(ex);
    failure.Message.Should().Be(ex.Message);
  }

  [Fact]
  public void CanCreateNotFoundFailure()
  {
    NotFoundFailure failure = new NotFoundFailure("Not found", "ID");
    failure.Id.Should().Be("ID");
    failure.Message.Should().Be("Not found");
  }

  [Fact]
  public void CanRecreateNotFoundFailure()
  {
    NotFoundFailure failure = new NotFoundFailure(new NotFoundFailure("Not found", "ID"));
    failure.Message.Should().Be("Not found");
    failure.Id.Should().Be("ID");
  }

  [Fact]
  public void CanCreateValidationFailure()
  {
    ValidationError error = new("ID", "Invalid");

    ValidationFailure failure = new ValidationFailure([error]);

    failure.Errors.Should().Contain(error);
    failure.Errors.Count().Should().Be(1);
    failure.Message.Should().Be("One or more validation errors occurred.");
  }

  [Fact]
  public void CanRecreateValidationFailure()
  {
    ValidationError error = new("ID", "Invalid");

    ValidationFailure failure = new ValidationFailure(new ValidationFailure([error]));

    failure.Errors.Should().Contain(error);
    failure.Errors.Count().Should().Be(1);
    failure.Message.Should().Be("One or more validation errors occurred.");
  }

  [Fact]
  public void CanCreateValidationFailureWithSingleError()
  {
    ValidationError error = new("ID", "Invalid");

    ValidationFailure failure = ValidationFailure.SingleFailure(error);

    failure.Errors.Should().Contain(error);
    failure.Errors.Count().Should().Be(1);
    failure.Message.Should().Be("One or more validation errors occurred.");
  }

  [Fact]
  public void CanCreateValidationFailureWithSinglePropertyError()
  {
    ValidationFailure failure = ValidationFailure.SingleFailure("ID", "Invalid");
    failure.Errors.Should().Contain(new ValidationError("ID", "Invalid"));
    failure.Errors.Count().Should().Be(1);
    failure.Message.Should().Be("One or more validation errors occurred.");
  }
}
