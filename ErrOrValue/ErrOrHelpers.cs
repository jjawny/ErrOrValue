using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ErrOrValue;

public static class ErrOrHelpers
{
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
  /// Merge the Status Code, Messages, and Value (if types match) from another ErrOr into this ErrOr
  /// </summary>
  public static ErrOr<TTarget> MergeWith<TTarget, TSource>(this ErrOr<TTarget> errOr, ErrOr<TSource> otherErrOr)
  {
    // Merge base properties
    ((ErrOr)errOr).MergeWith(otherErrOr);

    // If the types match, try to update the value
    if (otherErrOr is ErrOr<TTarget> sameTypeErrOr && sameTypeErrOr.Value != null)
    {
      errOr.Value = sameTypeErrOr.Value;
    }

    return errOr;
  }

  /// <summary>
  /// Merge the Status Code, Messages from another ErrOr into this ErrOr
  /// </summary>
  public static ErrOr MergeWith<TSource>(this ErrOr errOr, ErrOr<TSource> otherErrOr)
  {
    errOr.Code = otherErrOr.Code;
    errOr.Messages.AddRange(otherErrOr.Messages);
    return errOr;
  }

  /// <summary>
  /// Merge the Status Code, Messages from another ErrOr into this ErrOr
  /// </summary>
  public static ErrOr<TTarget> MergeWith<TTarget>(this ErrOr<TTarget> errOr, ErrOr otherErrOr)
  {
    // Merge base properties
    ((ErrOr)errOr).MergeWith(otherErrOr);
    return errOr;
  }

  /// <summary>
  /// Set all values at once
  /// </summary>
  public static ErrOr Set(
    this ErrOr errOr,
    string message,
    Severity severity,
    HttpStatusCode? code = null,
    Exception? ex = null)
  {
    errOr.AddMessage(message, severity);

    if (code.HasValue)
    {
      errOr.Code = code.Value;
    }

    if (ex != null)
    {
      var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
      var exceptionMessage = isDevelopment
        ? "Something went wrong..."
        : ex.InnerException == null
          ? ex.Message
          : ex.InnerException.Message;

      errOr.AddMessage(exceptionMessage, Severity.Error);
    }

    return errOr;
  }

  /// <summary>
  /// Set all values at once
  /// </summary>
  public static ErrOr<T> Set<T>(
    this ErrOr<T> errOr,
    string message,
    Severity severity,
    HttpStatusCode? code = null,
    Exception? ex = null,
    T? value = default)
  {
    errOr.Set(message, severity, code, ex);

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
  /// Map an ErrOr to an HTTP response for MVC controllers
  /// </summary>
  public static Microsoft.AspNetCore.Mvc.IActionResult ToApiResponse(this ErrOr errOr, object? value = null)
  {
    var body = GetApiResponsePayload(errOr, value);
    var res = new Microsoft.AspNetCore.Mvc.JsonResult(body) { StatusCode = (int)errOr.Code };
    return res;
  }

  /// <summary>
  /// Map an ErrOr to an HTTP response for MVC controllers
  /// </summary>
  public static Microsoft.AspNetCore.Mvc.IActionResult ToApiResponse<T>(this ErrOr<T> errOr) => errOr.ToApiResponse(errOr.Value);

  /// <summary>
  /// Map an ErrOr to an HTTP response for minimal APIs
  /// </summary>
  public static Microsoft.AspNetCore.Http.IResult ToMinimalApiResponse(this ErrOr errOr, object? value = null)
  {
    var body = GetApiResponsePayload(errOr, value);
    var res = Microsoft.AspNetCore.Http.Results.Json(body, statusCode: (int)errOr.Code);
    return res;
  }

  /// <summary>
  /// Map an ErrOr to an HTTP response for minimal APIs
  /// </summary>
  public static Microsoft.AspNetCore.Http.IResult ToMinimalApiResponse<T>(this ErrOr<T> errOr) => errOr.ToMinimalApiResponse(errOr.Value);

  /// <summary>
  /// Debug utility to log an ErrOr as JSON to the console
  /// </summary>
  public static void LogToConsole(this ErrOr errOr, bool isIgnoreMessages = false)
  {
    BaseLogToConsole(errOr, isIgnoreMessages: isIgnoreMessages);
  }

  /// <summary>
  /// Debug utility to log an ErrOr as JSON to the console
  /// </summary>
  public static void LogToConsole<T>(this ErrOr<T> errOr, bool isIgnoreMessages = false, bool isIgnoreValue = false)
  {
    BaseLogToConsole(errOr, errOr.Value, isIgnoreMessages: isIgnoreMessages, isIgnoreValue: isIgnoreValue);
  }

  /// <summary>
  /// Internal helper method to log an ErrOr as JSON to the console
  /// </summary>
  private static void BaseLogToConsole(ErrOr errOr, object? value = null, bool isIgnoreMessages = false, bool isIgnoreValue = false)
  {
    var prettifiedObject = new Dictionary<string, object>
    {
      ["IsOk"] = errOr.IsOk,
      ["Code"] = errOr.Code,
    };

    if (!isIgnoreMessages)
      prettifiedObject["Messages"] = errOr.Messages.Select(m => new { Message = m.Message, Severity = m.Severity.GetEnumDescription() }).ToList();

    if (!isIgnoreValue)
      prettifiedObject["Value"] = value ?? "null";

    var json = JsonSerializer.Serialize(prettifiedObject, new JsonSerializerOptions() { WriteIndented = true });

    Console.WriteLine(json);
  }

  /// <summary>
  /// Get the description attribute of an enum value or fallback to the enum as a string
  /// </summary>
  private static string GetEnumDescription(this Enum enumValue)
  {
    var field = enumValue.GetType().GetField(enumValue.ToString());

    if (field != null && Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute)) is System.ComponentModel.DescriptionAttribute attribute)
    {
      return attribute.Description;
    }

    return enumValue.ToString();
  }

  /// <summary>
  /// Creates a standardized response payload from an ErrOr
  /// </summary>
  private static object GetApiResponsePayload(ErrOr errOr, object? value = null)
  {
    // Convert tuples to named properties for JSON serialization
    var jsonSerializableMessages = errOr.Messages.Select(m => new { message = m.Message, severity = m.Severity.GetEnumDescription() }).ToList();

    return value != null
      ? new { value = value, messages = jsonSerializableMessages }
      : new { messages = jsonSerializableMessages };
  }
}
