using System.ComponentModel;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace ErrOrValue;

public static class ErrOrHelpers
{
  /// <summary>
  /// Get all error messages
  /// </summary>
  public static List<string> Errors(this ErrOr errOr)
  {
    return errOr.Messages
      .Where(m => m.Severity == Severity.Error)
      .Select(m => m.Message)
      .ToList();
  }

  /// <summary>
  /// Add a message
  /// </summary>
  public static ErrOr AddMessage(this ErrOr errOr, string message, Severity severity = Severity.Info)
  {
    errOr.Messages.Add((message, severity));

    return errOr;
  }

  /// <summary>
  /// Add many messages
  /// </summary>
  public static ErrOr AddMessages(this ErrOr errOr, IEnumerable<(string Message, Severity Severity)> messages)
  {
    errOr.Messages.AddRange(messages);

    return errOr;
  }

  /// <summary>
  /// Merge the Status Code and Messages from another ErrOr into this ErrOr
  /// </summary>
  public static ErrOr MergeWith(this ErrOr errOr, ErrOr otherErrOr)
  {
    errOr.Code = otherErrOr.Code;
    errOr.Messages.AddRange(otherErrOr.Messages);

    return errOr;
  }

  /// <summary>
  /// Merge the Status Code, Messages, and Value from another ErrOr into this ErrOr
  /// </summary>
  public static ErrOr<T> MergeWith<T>(this ErrOr<T> errOr, ErrOr<T> otherErrOr)
  {
    ((ErrOr)errOr).MergeWith(otherErrOr);

    if (otherErrOr.Value != null)
    {
      errOr.Value = otherErrOr.Value;
    }

    return errOr;
  }

  /// <summary>
  /// Set all values at once
  /// </summary>
  public static ErrOr Set(this ErrOr errOr, string? message = null, Severity severity = Severity.Info, HttpStatusCode statusCode = HttpStatusCode.OK, Exception? exception = null)
  {
    if (!string.IsNullOrWhiteSpace(message))
    {
      errOr.AddMessage(message, severity);
    }

    var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

    if (isDevelopment && exception != null)
    {
      var exceptionMessage = exception.InnerException == null ? exception.Message : exception.InnerException.Message;

      if (!string.IsNullOrWhiteSpace(exceptionMessage))
      {
        errOr.AddMessage(exceptionMessage, Severity.Error);
      }
    }

    errOr.Code = statusCode;

    return errOr;
  }

  /// <summary>
  /// Set all values at once
  /// </summary>
  public static ErrOr<T> Set<T>(this ErrOr<T> errOr, T? value = default, string? message = null, Severity severity = Severity.Info, HttpStatusCode statusCode = HttpStatusCode.OK, Exception? exception = null)
  {
    errOr.Set(message, severity, statusCode, exception);

    if (value != null)
    {
      errOr.Value = value;
    }

    return errOr;
  }

  /// <summary>
  /// Map a HTTP response to an ErrOr 
  /// </summary>
  public static ErrOr FromHttpResponse(this ErrOr errOr, HttpResponseMessage httpResponse, string externalServiceName)
  {
    errOr.Code = httpResponse.StatusCode;

    if (!httpResponse.IsSuccessStatusCode)
    {
      errOr.AddMessage($"Error from {externalServiceName}: {httpResponse.ReasonPhrase}", Severity.Error);
    }

    return errOr;
  }

  /// <summary>
  /// Map a HTTP response to an ErrOr 
  /// </summary>
  public static async Task<ErrOr<T>> FromHttpResponse<T>(this ErrOr<T> errOr, HttpResponseMessage httpResponse, string externalServiceName)
  {
    ((ErrOr)errOr).FromHttpResponse(httpResponse, externalServiceName);

    if (httpResponse.Content != null)
    {
      errOr.Value = await httpResponse.Content.ReadFromJsonAsync<T>();
    }

    return errOr;
  }

  /// <summary>
  /// Map an ErrOr to a HTTP response 
  /// </summary>
  public static IActionResult ToApiResponse(this ErrOr errOr, object? value = null)
  {
    // Convert tuples to named properties for JSON serialization
    var jsonSerializableMessages = errOr.Messages.Select(m => new { message = m.Message, severity = m.Severity.GetEnumDescription() }).ToList();

    object body = value != null
      ? new { value = value, messages = jsonSerializableMessages }
      : new { messages = jsonSerializableMessages };

    var apiRes = new JsonResult(body) { StatusCode = (int)errOr.Code };

    return apiRes;
  }

  /// <summary>
  /// Map an ErrOr to a HTTP response 
  /// </summary>
  public static IActionResult ToApiResponse<T>(this ErrOr<T> errOr) => errOr.ToApiResponse(errOr.Value);

  /// <summary>
  /// Get the description attribute of an enum value or fallback to the enum as a string
  /// </summary>
  private static string GetEnumDescription(this Enum enumValue)
  {
    var field = enumValue.GetType().GetField(enumValue.ToString());

    if (field != null && Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
    {
      return attribute.Description;
    }

    return enumValue.ToString();
  }
}
