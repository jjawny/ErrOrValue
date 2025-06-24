using Xunit;

namespace ErrOrValue.Tests;

public class ErrorTests
{
  [Fact]
  public void GetErrorMessagesOnly()
  {
    // Arrange
    var errOr = new ErrOr();
    var messagesToAdd = new[]
    {
      ("Info", Severity.Info),
      ("Warning", Severity.Warning),
      ("Error", Severity.Error),
    };
    errOr.AddMessages(messagesToAdd);

    // Act
    var errors = errOr.Errors;

    // Assert
    Assert.Contains("Error", errors);
    Assert.DoesNotContain("Info", errors);
    Assert.DoesNotContain("Warning", errors);
  }

  [Fact]
  public void GetValue()
  {
    // Arrange
    var errOr = new ErrOr<string>();
    var testValue = "Test Value";

    // Act
    errOr.Value = testValue;

    // Assert
    Assert.NotNull(errOr.Value);
    Assert.Equal(testValue, errOr.Value);
  }
}
