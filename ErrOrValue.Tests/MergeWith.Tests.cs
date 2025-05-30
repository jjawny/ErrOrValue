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
  }

  [Fact]
  public void MergeWith_MergesValueOfSameType()
  {
    // Arrange
    var errOr1 = new ErrOr<string> { Value = "OriginalValue" };
    var errOr2 = new ErrOr<string> { Value = "UpdatedValue" };

    // Act
    errOr1.MergeWith(errOr2);

    // Assert
    Assert.Equal(errOr2.Value, errOr1.Value);
  }

  [Fact]
  public void MergeWith_IgnoresMergingValueOfSameTypeButNotYetSet()
  {
    // Arrange
    var errOr1 = new ErrOr<string> { Value = "Original" };
    var errOr2 = new ErrOr<string> { Value = null }; // value not set yet

    // Act
    errOr1.MergeWith(errOr2);

    // Assert
    Assert.NotNull(errOr1.Value);
  }

  [Fact]
  public void MergeWith_IgnoresMergineValuesOfDifferentTypes()
  {
    // Arrange
    var errOr1 = new ErrOr<int> { Value = 1 };
    var errOr2 = new ErrOr<string> { Value = "One" };

    // Act
    errOr1.MergeWith(errOr2);

    // Assert
    Assert.Equal(1, errOr1.Value);
  }
}
