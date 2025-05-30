using Xunit;

namespace ErrOrValue.Tests;

public class ErrorTests
{
  [Fact]
  public void GetErrorMessagesOnly_Success()
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
    var errors = errOr.Errors();

    // Assert
    Assert.Equal(messagesToAdd.Length, errors.Count);
    Assert.Contains("Error", errors);
    Assert.DoesNotContain("Info", errors);
    Assert.DoesNotContain("Warning", errors);
  }
}
