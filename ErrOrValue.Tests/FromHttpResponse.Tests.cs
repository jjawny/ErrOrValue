using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ErrOrValue.Tests;

public class FromHttpResponseTests
{
  [Theory]
  [InlineData(HttpStatusCode.OK, true)]
  [InlineData(HttpStatusCode.NoContent, true)]
  [InlineData(HttpStatusCode.BadRequest, false)]
  [InlineData(HttpStatusCode.NotFound, false)]
  public void MapsStatusWithoutReadingBody(HttpStatusCode code, bool succeeds)
  {
    using var response = new HttpResponseMessage(code);
    var result = response.FromHttpResponse("Service");
    Assert.Equal(succeeds, result is Success);
    if (result is Success success)
      Assert.Equal(code, success.Code);
    if (result is Error error)
      Assert.Equal(code, error.Code);
  }

  [Fact]
  public async Task ReadsSuccessfulJsonAndRetainsStatus()
  {
    using var response = new HttpResponseMessage(HttpStatusCode.Created)
    {
      Content = JsonContent.Create(new Dto(1, "Test"))
    };
    var result = await response.FromHttpResponse<Dto>("Service");
    Assert.True(result is Success<Dto> { Value: { Id: 1, Name: "Test" }, Code: HttpStatusCode.Created });
  }

  [Fact]
  public async Task DoesNotDeserializeErrorBodyAsSuccess()
  {
    using var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
    {
      Content = new StringContent("not JSON"),
      ReasonPhrase = "Bad request"
    };
    var result = await response.FromHttpResponse<Dto>("Service");
    Assert.True(result is Error { Code: HttpStatusCode.BadRequest, Message: "Error from Service: Bad request" });
  }

  [Theory]
  [InlineData("not JSON")]
  [InlineData("")]
  [InlineData("null")]
  public async Task InvalidOrEmptySuccessBodyBecomesBadGateway(string body)
  {
    using var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body) };
    var result = await response.FromHttpResponse<Dto>("Service");
    Assert.True(result is Error { Code: HttpStatusCode.BadGateway });
  }

  [Fact]
  public async Task CancellationPropagates()
  {
    using var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(42) };
    using var cancellation = new CancellationTokenSource();
    cancellation.Cancel();
    await Assert.ThrowsAnyAsync<OperationCanceledException>(() => response.FromHttpResponse<int>("Service", cancellation.Token));
  }

  public record Dto(int Id, string Name);
}
