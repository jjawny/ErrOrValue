using System.Net;
using System.Net.Http.Json;
using Moq;
using Moq.Protected;
using Xunit;

namespace ErrOrValue.Tests;

public class FromHttpResponseTests
{
  [Theory]
  [InlineData(HttpStatusCode.OK, "Service is healthy")]
  [InlineData(HttpStatusCode.BadRequest, "Invalid data")]
  [InlineData(HttpStatusCode.NotFound, "Resource not found")]
  public void FromHttpResponse_SetsCodeAndAddsErrorMessageIfNotSuccess(HttpStatusCode statusCode, string reasonPhrase)
  {
    // Arrange
    var errOr = new ErrOr();
    var httpResponse = new HttpResponseMessage
    {
      StatusCode = statusCode,
      ReasonPhrase = reasonPhrase
    };

    // Act
    errOr.FromHttpResponse(httpResponse, "TestService");

    // Assert
    Assert.Equal(statusCode, errOr.Code);

    if ((int)statusCode >= 400)
    {
      Assert.Single(errOr.Messages);
      Assert.Equal($"Error from TestService: {reasonPhrase}", errOr.Messages[0].Message);
      Assert.Equal(Severity.Error, errOr.Messages[0].Severity);
    }
    else
    {
      Assert.Empty(errOr.Messages);
    }
  }

  [Fact]
  public async Task FromHttpResponseGeneric_SetsValueFromJsonContent()
  {
    // Arrange
    var errOr = new ErrOr<ExampleValue>();
    var testModel = new ExampleValue { Id = 1, Name = "Test" };

    var handlerMock = new Mock<HttpMessageHandler>();
    handlerMock
        .Protected()
        .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        )
        .ReturnsAsync(new HttpResponseMessage
        {
          StatusCode = HttpStatusCode.OK,
          Content = JsonContent.Create(testModel)
        });

    var httpClient = new HttpClient(handlerMock.Object);
    var httpResponse = await httpClient.GetAsync("https://example.com/api/data");

    // Act
    await errOr.FromHttpResponse(httpResponse, "TestService");

    // Assert
    Assert.Equal(HttpStatusCode.OK, errOr.Code);
    Assert.NotNull(errOr.Value);
    Assert.Equal(1, errOr.Value.Id);
    Assert.Equal("Test", errOr.Value.Name);
  }

  // Model for testing JSON deserialization
  private class ExampleValue
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
  }
}
