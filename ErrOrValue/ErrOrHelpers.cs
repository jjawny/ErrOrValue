using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErrOrValue;

public static class ErrOrHelpers
{
  /// <summary>Maps an HTTP status to a success or error without reading its body.</summary>
  public static ErrOr FromHttpResponse(this HttpResponseMessage response, string externalServiceName) =>
    response.IsSuccessStatusCode
      ? new Success(response.StatusCode)
      : HttpError(response, externalServiceName);

  /// <summary>Reads a successful JSON response; error bodies aren't deserialized as T.</summary>
  public static async Task<ErrOr<T>> FromHttpResponse<T>(
    this HttpResponseMessage response,
    string externalServiceName,
    CancellationToken cancellationToken = default)
  {
    if (!response.IsSuccessStatusCode)
      return HttpError(response, externalServiceName);

    try
    {
      var value = await response.Content.ReadFromJsonAsync<T>(cancellationToken);
      return value is null
        ? new Error($"Empty JSON value from {externalServiceName}", HttpStatusCode.BadGateway)
        : new Success<T>(value, response.StatusCode);
    }
    catch (JsonException)
    {
      return new Error($"Invalid JSON from {externalServiceName}", HttpStatusCode.BadGateway);
    }
  }

  public static IActionResult ToApiResponse(this ErrOr result)
  {
    var (body, code) = GetResponse(result);
    return new JsonResult(body) { StatusCode = (int)code };
  }

  public static IActionResult ToApiResponse<T>(this ErrOr<T> result)
  {
    var (body, code) = GetResponse(result);
    return new JsonResult(body) { StatusCode = (int)code };
  }

  public static IResult ToMinimalApiResponse(this ErrOr result)
  {
    var (body, code) = GetResponse(result);
    return Results.Json(body, statusCode: (int)code);
  }

  public static IResult ToMinimalApiResponse<T>(this ErrOr<T> result)
  {
    var (body, code) = GetResponse(result);
    return Results.Json(body, statusCode: (int)code);
  }

  private static Error HttpError(HttpResponseMessage response, string service) =>
    new($"Error from {service}: {response.ReasonPhrase}", response.StatusCode);

  private static (object Body, HttpStatusCode Code) GetResponse(ErrOr result) => result switch
  {
    Success success => (Payload(success.Messages), success.Code),
    Error error => ErrorResponse(error),
    null => throw new InvalidOperationException("ErrOr has no outcome. Return a Success or Error case."),
  };

  private static (object Body, HttpStatusCode Code) GetResponse<T>(ErrOr<T> result) => result switch
  {
    Success<T> success => (new { value = success.Value, messages = JsonMessages(success.Messages) }, success.Code),
    Error error => ErrorResponse(error),
    null => throw new InvalidOperationException("ErrOr has no outcome. Return a Success<T> or Error case."),
  };

  private static (object Body, HttpStatusCode Code) ErrorResponse(Error error) =>
    (Payload([.. error.Messages, (error.Message, Severity.Error)]), error.Code);

  private static object Payload(IReadOnlyList<(string Message, Severity Severity)> messages) =>
    new { messages = JsonMessages(messages) };

  private static object JsonMessages(IReadOnlyList<(string Message, Severity Severity)> messages) =>
    messages.Select(m => new { message = m.Message, severity = m.Severity.ToString().ToLowerInvariant() }).ToArray();
}
