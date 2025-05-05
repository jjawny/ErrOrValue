using System.Net;
using Xunit;

namespace ErrOrValue.Tests;

public class AddMessagesTests
{
  [Theory]
  [InlineData("Test message", Severity.Info)]
  [InlineData("Error occurred", Severity.Error)]
  [InlineData("Warning notice", Severity.Warning)]
  public void AddMessage_AddsMessageWithSeverity(string message, Severity severity)
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
  public void AddMessage_DefaultSeverityIsInfo()
  {
    // Arrange
    var errOr = new ErrOr();

    // Act
    errOr.AddMessage("Test message");

    // Assert
    Assert.Single(errOr.Messages);
    Assert.Equal("Test message", errOr.Messages[0].Message);
    Assert.Equal(Severity.Info, errOr.Messages[0].Severity);
  }

  [Fact]
  public void AddMessages_AddsMultipleMessages()
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
    Assert.Equal(3, errOr.Messages.Count);
    Assert.Contains(errOr.Messages, m => m.Message == "Info message" && m.Severity == Severity.Info);
    Assert.Contains(errOr.Messages, m => m.Message == "Error message" && m.Severity == Severity.Error);
    Assert.Contains(errOr.Messages, m => m.Message == "Warning message" && m.Severity == Severity.Warning);
  }
}
