using System.Net;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ErrOrValue.Tests;

public class ToApiResponseTests
{
  [Theory]
  [InlineData(HttpStatusCode.OK)]
  [InlineData(HttpStatusCode.Created)]
  [InlineData(HttpStatusCode.BadRequest)]
  [InlineData(HttpStatusCode.NotFound)]
  public void ToApiResponse_ReturnsJsonResultWithCorrectStatusCode(HttpStatusCode statusCode)
  {
    // Arrange
    var errOr = new ErrOr { Code = statusCode };
    errOr.AddMessage("Test message", Severity.Info);

    // Act
    var result = errOr.ToApiResponse();

    // Assert
    var jsonResult = Assert.IsType<JsonResult>(result);
    Assert.Equal((int)statusCode, jsonResult.StatusCode);
  }

  [Fact]
  public void ToApiResponseGeneric_IncludesValueInResponse()
  {
    // Arrange
    var errOr = new ErrOr<string> { Value = "Test value" };
    errOr.AddMessage("Test message", Severity.Info);

    // Act
    var result = errOr.ToApiResponse();

    // Assert
    var jsonResult = Assert.IsType<JsonResult>(result);

    Assert.Equal(200, jsonResult.StatusCode);
  }

  [Theory]
  [InlineData(HttpStatusCode.OK)]
  [InlineData(HttpStatusCode.Created)]
  [InlineData(HttpStatusCode.BadRequest)]
  [InlineData(HttpStatusCode.NotFound)]
  public void ToMinimalApiResponse_ReturnsResultWithCorrectStatusCode(HttpStatusCode statusCode)
  {
    // Arrange
    var errOr = new ErrOr { Code = statusCode };
    errOr.AddMessage("Test message", Severity.Info);

    // Act
    var result = errOr.ToMinimalApiResponse();

    // Assert
    Assert.NotNull(result);
  }

  [Fact]
  public void ToMinimalApiResponseGeneric_IncludesValueInResponse()
  {
    // Arrange
    var errOr = new ErrOr<int> { Value = 42 };
    errOr.AddMessage("Test message", Severity.Info);

    // Act
    var result = errOr.ToMinimalApiResponse();

    // Assert
    Assert.NotNull(result);
  }
}
