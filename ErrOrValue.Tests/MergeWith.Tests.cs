using System.Net;
using Xunit;

namespace ErrOrValue.Tests;

public class MergeWithTests
{
  [Fact]
  public void MergeWith_MergesStatusCodeAndMessages()
  {
    // Arrange
    var errOr1 = new ErrOr { Code = HttpStatusCode.OK };
    errOr1.AddMessage("Message 1", Severity.Info);

    var errOr2 = new ErrOr { Code = HttpStatusCode.BadRequest };
    errOr2.AddMessage("Message 2", Severity.Error);

    // Act
    errOr1.MergeWith(errOr2);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, errOr1.Code);
    Assert.Equal(2, errOr1.Messages.Count);
    Assert.Contains(errOr1.Messages, m => m.Message == "Message 1" && m.Severity == Severity.Info);
    Assert.Contains(errOr1.Messages, m => m.Message == "Message 2" && m.Severity == Severity.Error);
  }

  [Fact]
  public void MergeWithGeneric_MergesValuesAndMessages()
  {
    // Arrange
    var errOr1 = new ErrOr<string> { Value = "Original" };
    errOr1.AddMessage("Message 1", Severity.Info);

    var errOr2 = new ErrOr<string> { Value = "Updated", Code = HttpStatusCode.NotFound };
    errOr2.AddMessage("Message 2", Severity.Error);

    // Act
    errOr1.MergeWith(errOr2);

    // Assert
    Assert.Equal("Updated", errOr1.Value);
    Assert.Equal(HttpStatusCode.NotFound, errOr1.Code);
    Assert.Equal(2, errOr1.Messages.Count);
  }

  [Fact]
  public void MergeWithGeneric_NullValue_DoesNotUpdateValue()
  {
    // Arrange
    var errOr1 = new ErrOr<string> { Value = "Original" };
    var errOr2 = new ErrOr<string> { Value = null };

    // Act
    errOr1.MergeWith(errOr2);

    // Assert
    Assert.Equal("Original", errOr1.Value);
  }

  [Fact]
  public void MergeWithDifferentTypes_OnlyMergesStatusCodeAndMessages()
  {
    // Arrange
    var errOr1 = new ErrOr<int> { Value = 42 };
    errOr1.AddMessage("Message 1", Severity.Info);

    var errOr2 = new ErrOr<string> { Value = "Test", Code = HttpStatusCode.NotFound };
    errOr2.AddMessage("Message 2", Severity.Error);

    // Act
    errOr1.MergeWith(errOr2);

    // Assert
    Assert.Equal(42, errOr1.Value);
    Assert.Equal(HttpStatusCode.NotFound, errOr1.Code);
    Assert.Equal(2, errOr1.Messages.Count);
  }
}
