using System.ComponentModel;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace ErrOrResult;

public static class ErrOrHelpers
{
  public static ErrOr AddMessage(this ErrOr errOr, string message, Severity severity = Severity.Info)
  {
    errOr.Messages.Add((message, severity));

    return errOr;
  }

  public static ErrOr MergeWith(this ErrOr errOr, ErrOr otherErrOr)
  {
    errOr.Code = otherErrOr.Code;
    errOr.Messages.AddRange(otherErrOr.Messages);

    return errOr;
  }

  public static ErrOr<T> MergeWith<T>(this ErrOr<T> errOr, ErrOr<T> otherErrOr)
  {
    ((ErrOr)errOr).MergeWith(otherErrOr);

    if (otherErrOr.Value != null)
    {
      errOr.Value = otherErrOr.Value;
    }

    return errOr;
  }

  public static ErrOr QuickReturn(this ErrOr errOr, string? message = null, Severity severity = Severity.Info, HttpStatusCode code = HttpStatusCode.OK, Exception? ex = null)
  {
    if (!string.IsNullOrWhiteSpace(message))
    {
      errOr.AddMessage(message, severity);
    }

    var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

    if (isDevelopment && ex != null)
    {
      var exMessage = ex.InnerException == null ? ex.Message : ex.InnerException.Message;

      if (!string.IsNullOrWhiteSpace(exMessage))
      {
        errOr.AddMessage(exMessage, Severity.Error);
      }
    }

    errOr.Code = code;

    return errOr;
  }

  public static ErrOr<T> QuickReturn<T>(this ErrOr<T> errOr, T? value = default, string? message = null, Severity severity = Severity.Info, HttpStatusCode code = HttpStatusCode.OK, Exception? ex = null)
  {
    errOr.QuickReturn(message, severity, code, ex);

    if (value != null)
    {
      errOr.Value = value;
    }

    return errOr;
  }

  public static ErrOr FromHttpResponse(this ErrOr errOr, HttpResponseMessage httpResponse, string externalServiceName)
  {
    errOr.Code = httpResponse.StatusCode;

    if (!httpResponse.IsSuccessStatusCode)
    {
      errOr.AddMessage($"Error from {externalServiceName}: {httpResponse.ReasonPhrase}", Severity.Error);
    }

    return errOr;
  }

  public static async Task<ErrOr<T>> FromHttpResponse<T>(this ErrOr<T> errOr, HttpResponseMessage httpRes, string externalServiceName)
  {
    ((ErrOr)errOr).FromHttpResponse(httpRes, externalServiceName);

    if (httpRes.Content != null)
    {
      errOr.Value = await httpRes.Content.ReadFromJsonAsync<T>();
    }

    return errOr;
  }

  public static IActionResult ToApiRes(this ErrOr errOr, object? value = null)
  {
    // Convert messages from tuples to object with named properties
    //  to ensure messages are serialized properly when returning as JSON
    var jsonSerializableMessages = errOr.Messages.Select(m => new { message = m.Message, severity = m.Severity.GetEnumDescription() }).ToList();

    object body = value != null
      ? new { value = value, messages = jsonSerializableMessages }
      : new { messages = jsonSerializableMessages };

    var apiRes = new JsonResult(body) { StatusCode = (int)errOr.Code };

    return apiRes;
  }

  public static IActionResult ToApiRes<T>(this ErrOr<T> errOr) => errOr.ToApiRes(errOr.Value);

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
