namespace ExpenseExplorer.Tests.Common;

using System.Net;
using FluentAssertions;
using FluentAssertions.Numeric;

public static class AssertionExtensions
{
  public static AndConstraint<NumericAssertions<int>> ShouldBeIn200Group(this HttpStatusCode statusCode)
  {
    return ((int)statusCode).Should().BeInRange(200, 299);
  }
}
