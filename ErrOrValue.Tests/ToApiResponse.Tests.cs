using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ErrOrValue.Tests;

public class ToApiResponseTests
{
  [Fact]
  public void MvcSuccessIncludesValueStatusAndMessages()
  {
    ErrOr<int> result = new Success<int>(42, HttpStatusCode.Created)
    {
      Messages = [("Created", Severity.Info), ("Check this", Severity.Warning)]
    };
    var response = Assert.IsType<JsonResult>(result.ToApiResponse());
    var body = JsonSerializer.SerializeToElement(response.Value);
    Assert.Equal(201, response.StatusCode);
    Assert.Equal(42, body.GetProperty("value").GetInt32());
    Assert.Equal("info", body.GetProperty("messages")[0].GetProperty("severity").GetString());
    Assert.Equal("warning", body.GetProperty("messages")[1].GetProperty("severity").GetString());
  }

  [Fact]
  public void ErrorIncludesPrimaryAndAdditionalMessagesWithoutValue()
  {
    ErrOr<int> result = new Error("Missing", HttpStatusCode.NotFound)
    {
      Messages = [("Lookup attempted", Severity.Info)]
    };
    var response = Assert.IsType<JsonResult>(result.ToApiResponse());
    var body = JsonSerializer.SerializeToElement(response.Value);
    Assert.Equal(404, response.StatusCode);
    Assert.False(body.TryGetProperty("value", out _));
    Assert.Equal(2, body.GetProperty("messages").GetArrayLength());
    Assert.Equal("Missing", body.GetProperty("messages")[1].GetProperty("message").GetString());
    Assert.Equal("error", body.GetProperty("messages")[1].GetProperty("severity").GetString());
  }

  [Fact]
  public void SuccessWithoutValueHasOnlyMessages()
  {
    ErrOr result = new Success(HttpStatusCode.Accepted);
    var response = Assert.IsType<JsonResult>(result.ToApiResponse());
    var body = JsonSerializer.SerializeToElement(response.Value);
    Assert.Equal(202, response.StatusCode);
    Assert.False(body.TryGetProperty("value", out _));
    Assert.Equal(0, body.GetProperty("messages").GetArrayLength());
  }

  [Fact]
  public void NullableSuccessIncludesExplicitNullValue()
  {
    ErrOr<string?> result = new Success<string?>(null);
    var response = Assert.IsType<JsonResult>(result.ToApiResponse());
    Assert.Equal(JsonValueKind.Null, JsonSerializer.SerializeToElement(response.Value).GetProperty("value").ValueKind);
  }

  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void MinimalApiMatchesMvcPayloadAndStatus(bool succeeds)
  {
    ErrOr<int> result = succeeds ? new Success<int>(42, HttpStatusCode.Created) : new Error("Missing", HttpStatusCode.NotFound);
    var mvc = Assert.IsType<JsonResult>(result.ToApiResponse());
    var minimal = result.ToMinimalApiResponse();
    Assert.Equal(mvc.StatusCode, Assert.IsAssignableFrom<IStatusCodeHttpResult>(minimal).StatusCode);
    Assert.Equal(JsonSerializer.Serialize(mvc.Value), JsonSerializer.Serialize(Assert.IsAssignableFrom<IValueHttpResult>(minimal).Value));
  }

  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void NonGenericMinimalApiMatchesMvc(bool succeeds)
  {
    ErrOr result = succeeds ? new Success() : new Error("Failed");
    var mvc = Assert.IsType<JsonResult>(result.ToApiResponse());
    var minimal = result.ToMinimalApiResponse();
    Assert.Equal(mvc.StatusCode, Assert.IsAssignableFrom<IStatusCodeHttpResult>(minimal).StatusCode);
    Assert.Equal(JsonSerializer.Serialize(mvc.Value), JsonSerializer.Serialize(Assert.IsAssignableFrom<IValueHttpResult>(minimal).Value));
  }

  [Fact]
  public void UninitializedUnionsFailClearly()
  {
    Assert.Throws<InvalidOperationException>(() => default(ErrOr).ToApiResponse());
    Assert.Throws<InvalidOperationException>(() => default(ErrOr<int>).ToApiResponse());
    Assert.Throws<InvalidOperationException>(() => default(ErrOr).ToMinimalApiResponse());
    Assert.Throws<InvalidOperationException>(() => default(ErrOr<int>).ToMinimalApiResponse());
  }
}
