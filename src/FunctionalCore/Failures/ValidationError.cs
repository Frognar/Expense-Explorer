namespace FunctionalCore.Failures;

public record ValidationError(string Property, string ErrorCode)
{
  public static ValidationError Create(string property, string errorCode) => new(property, errorCode);
}
