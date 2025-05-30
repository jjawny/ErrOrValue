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
  [InlineData(HttpStatusCode.BadRequest, "Bad DTO shape")]
  [InlineData(HttpStatusCode.NotFound, "Resource not found")]
  public void TransfersHttpResponseCode(HttpStatusCode statusCode, string reasonPhrase)
  {
    // Arrange
    var errOr = new ErrOr();
    var httpResponse = new HttpResponseMessage
    {
      StatusCode = statusCode,
      ReasonPhrase = reasonPhrase
    };

    // Act
    errOr.FromHttpResponse(httpResponse, "3rdPartyService");

    // Assert
    Assert.Equal(httpResponse.StatusCode, errOr.Code);

    if ((int)statusCode >= 400)
    {
      Assert.Single(errOr.Messages);
      Assert.Equal(Severity.Error, errOr.Messages[0].Severity);
    }
    else
    {
      Assert.Empty(errOr.Messages);
    }
  }

  [Fact]
  public async Task TransfersHttpResponseJsonBody()
  {
    // Arrange
    var errOr = new ErrOr<ExampleValueFromHttpResponse>();
    var exampleValueFromHttpResponse = new ExampleValueFromHttpResponse { Id = 1, Name = "Test" };

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
          Content = JsonContent.Create(exampleValueFromHttpResponse)
        });

    var httpClient = new HttpClient(handlerMock.Object);
    var httpResponse = await httpClient.GetAsync("https://example.com/api/data");

    // Act
    await errOr.FromHttpResponse(httpResponse, "3rdPartyService");

    // Assert
    Assert.Equal(HttpStatusCode.OK, errOr.Code);
    Assert.NotNull(errOr.Value);
    Assert.Equal(exampleValueFromHttpResponse.Id, errOr.Value.Id);
    Assert.Equal(exampleValueFromHttpResponse.Name, errOr.Value.Name);
  }

  private class ExampleValueFromHttpResponse
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
  }
}
