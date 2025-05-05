using System.Net;
using Xunit;

namespace ErrOrValue.Tests;

public class ErrorTests
{
  [Fact]
  public void Errors_ReturnsOnlyErrorMessages()
  {
    // Arrange
    var errOr = new ErrOr();
    errOr.AddMessage("Info message", Severity.Info);
    errOr.AddMessage("Warning message", Severity.Warning);
    errOr.AddMessage("Error message 1", Severity.Error);
    errOr.AddMessage("Error message 2", Severity.Error);

    // Act
    var errors = errOr.Errors();

    // Assert
    Assert.Equal(2, errors.Count);
    Assert.Contains("Error message 1", errors);
    Assert.Contains("Error message 2", errors);
    Assert.DoesNotContain("Info message", errors);
    Assert.DoesNotContain("Warning message", errors);
  }

  [Fact]
  public void Errors_WithNoErrorMessages_ReturnsEmptyList()
  {
    // Arrange
    var errOr = new ErrOr();
    errOr.AddMessage("Info message", Severity.Info);
    errOr.AddMessage("Warning message", Severity.Warning);

    // Act
    var errors = errOr.Errors();

    // Assert
    Assert.Empty(errors);
  }
}
