using System.Net;
using Xunit;

namespace ErrOrValue.Tests;

public class MergeWithTests
{
  [Fact]
  public void MergeBothNonGeneric()
  {
    // Arrange
    var errOr = new ErrOr();
    var otherErrOr = new ErrOr { Code = HttpStatusCode.BadRequest };
    otherErrOr.AddMessage("Some message", Severity.Info);

    // Act
    errOr.MergeWith(otherErrOr);

    // Assert
    Assert.Equal(otherErrOr.Code, errOr.Code);
    Assert.Single(errOr.Messages);
  }

  [Fact]
  public void MergeBothGeneric()
  {
    // Arrange
    var errOr = new ErrOr<string> { Value = "OriginalValue" };
    var otherErrOr = new ErrOr<string> { Value = "UpdatedValue", Code = HttpStatusCode.BadRequest };
    otherErrOr.AddMessage("Some message", Severity.Info);

    // Act
    errOr.MergeWith(otherErrOr);

    // Assert
    Assert.Equal(otherErrOr.Value, errOr.Value);
    Assert.Equal(otherErrOr.Code, errOr.Code);
    Assert.Single(errOr.Messages);
  }

  [Fact]
  public void MergeBothGenericButValueNotSetYet()
  {
    // Arrange
    var errOr = new ErrOr<string> { Value = "Original" };
    var otherErrOr = new ErrOr<string> { Value = null, Code = HttpStatusCode.BadRequest }; // value not set yet
    otherErrOr.AddMessage("Some message", Severity.Info);

    // Act
    errOr.MergeWith(otherErrOr);

    // Assert
    Assert.NotNull(errOr.Value);
    Assert.Equal(otherErrOr.Code, errOr.Code);
    Assert.Single(errOr.Messages);
  }

  [Fact]
  public void MergeBothGenericButDifferentTypes()
  {
    // Arrange
    var errOr = new ErrOr<int> { Value = 1 };
    var otherErrOr = new ErrOr<string> { Value = "one", Code = HttpStatusCode.BadRequest };
    otherErrOr.AddMessage("Some message", Severity.Info);

    // Act
    errOr.MergeWith(otherErrOr);

    // Assert
    Assert.Equal(1, errOr.Value);
    Assert.Equal(otherErrOr.Code, errOr.Code);
    Assert.Single(errOr.Messages);
  }

  [Fact]
  public void MergeGenericWithNonGeneric()
  {
    // Arrange
    var errOr = new ErrOr();
    var otherErrOr = new ErrOr<string> { Value = "one", Code = HttpStatusCode.BadRequest };
    otherErrOr.AddMessage("Some message", Severity.Info);

    // Act
    var result = errOr.MergeWith(otherErrOr);

    // Assert
    Assert.IsType<ErrOr>(result);
    Assert.Equal(otherErrOr.Code, result.Code);
    Assert.Single(errOr.Messages);
  }

  [Fact]
  public void MergeNonGenericWithGeneric()
  {
    // Arrange
    var errOr = new ErrOr<int> { Value = 1 };
    var otherErrOr = new ErrOr { Code = HttpStatusCode.BadRequest };
    otherErrOr.AddMessage("Some message", Severity.Info);

    // Act
    var result = errOr.MergeWith(otherErrOr);

    // Assert
    Assert.IsType<ErrOr<int>>(result);
    Assert.Equal(otherErrOr.Code, result.Code);
    Assert.Single(errOr.Messages);
  }
}
