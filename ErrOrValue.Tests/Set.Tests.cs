using System.Net;
using Xunit;

namespace ErrOrValue.Tests;

public class SetTests
{
  [Theory]
  [InlineData("Test message", Severity.Info, HttpStatusCode.OK)]
  [InlineData("Error message", Severity.Error, HttpStatusCode.BadRequest)]
  [InlineData("Warning message", Severity.Warning, HttpStatusCode.NotFound)]
  public void SetsErrOrProperties(string message, Severity severity, HttpStatusCode code)
  {
    // Arrange
    var errOr = new ErrOr();

    // Act
    errOr.Set(message, severity, code);

    // Assert
    Assert.Equal(code, errOr.Code);
    Assert.Single(errOr.Messages);
    Assert.Equal(message, errOr.Messages[0].Message);
    Assert.Equal(severity, errOr.Messages[0].Severity);
  }

  [Fact]
  public void Set_WithException_AddsExceptionMessageInDevelopment()
  {
    // Arrange
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
    var errOr = new ErrOr();
    var exceptionMessage = "Test exception";
    var exception = new Exception(exceptionMessage);

    // Act
    errOr.Set(
      message: "Bad request",
      severity: Severity.Error,
      ex: exception);

    // Assert
    Assert.Single(errOr.Messages);
    Assert.Equal(exceptionMessage, errOr.Messages[0].Message);
    Assert.Equal(Severity.Error, errOr.Messages[0].Severity);

    // Cleanup
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
  }

  [Fact]
  public void Set_WithNestedException_AddsInnerExceptionMessageInDevelopment()
  {
    // Arrange
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
    var errOr = new ErrOr();
    var innerExceptionMessage = "Inner exception";
    var exception = new Exception("Outer exception", new Exception(innerExceptionMessage));

    // Act
    errOr.Set(
      message: "Bad request",
      severity: Severity.Error,
      ex: exception);

    // Assert
    Assert.Single(errOr.Messages);
    Assert.Equal(innerExceptionMessage, errOr.Messages[0].Message);
    Assert.Equal(Severity.Error, errOr.Messages[0].Severity);

    // Cleanup
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
  }

  [Theory]
  [InlineData(42, "Test message", Severity.Info, HttpStatusCode.OK)]
  [InlineData("Test", "Error message", Severity.Error, HttpStatusCode.BadRequest)]
  [InlineData(true, "Warning message", Severity.Warning, HttpStatusCode.NotFound)]
  public void SetGeneric_SetsValueAndErrOrProperties<T>(T value, string message, Severity severity, HttpStatusCode code)
  {
    // Arrange
    var errOr = new ErrOr<T>();

    // Act
    errOr.Set(message, severity, code, value: value);

    // Assert
    Assert.Equal(value, errOr.Value);
    Assert.Equal(code, errOr.Code);
    Assert.Single(errOr.Messages);
    Assert.Equal(message, errOr.Messages[0].Message);
    Assert.Equal(severity, errOr.Messages[0].Severity);
  }
}
