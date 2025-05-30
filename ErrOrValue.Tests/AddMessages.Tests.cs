using System.Net;
using Xunit;

namespace ErrOrValue.Tests;

public class AddMessagesTests
{
  [Theory]
  [InlineData("Info message", Severity.Info)]
  [InlineData("Error message", Severity.Error)]
  [InlineData("Warning message", Severity.Warning)]
  public void AddMessages(string message, Severity severity)
  {
    // Arrange
    var errOr = new ErrOr();

    // Act
    errOr.AddMessage(message, severity);

    // Assert
    Assert.Single(errOr.Messages);
    Assert.Equal(message, errOr.Messages[0].Message);
    Assert.Equal(severity, errOr.Messages[0].Severity);
  }

  [Fact]
  public void AddMessageWithDefaultSeverity()
  {
    // Arrange
    var errOr = new ErrOr();

    // Act
    errOr.AddMessage("Test message");

    // Assert
    Assert.Equal(Severity.Info, errOr.Messages[0].Severity);
  }

  [Fact]
  public void AddManyMessages()
  {
    // Arrange
    var errOr = new ErrOr();
    var messages = new List<(string Message, Severity Severity)>
    {
        ("Info message", Severity.Info),
        ("Error message", Severity.Error),
        ("Warning message", Severity.Warning)
    };

    // Act
    errOr.AddMessages(messages);

    // Assert
    Assert.Equal(messages.Count, errOr.Messages.Count);
  }
}
