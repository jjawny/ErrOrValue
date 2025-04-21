# ErrOrResult

My take on _errors as values_ in .NET

### Example usage

```csharp

  var res = new ErrOr<bool>()

  // Happy path
  return res.QuickReturn(
      value: true,
      message: $"Successfull op",
      severity: Severity.Info,
      code: HttpStatusCode.OK);

  // Issues
  return res.QuickReturn(
    value: false,
    message: $"Issue with op",
    severity: Severity.Warning,
    code: HttpStatusCode.NotFound);

  // Exceptions
  catch (Exception ex)
  {
    return res.QuickReturn(
        message: "Something went wrong...",
        severity: Severity.Error,
        code: HttpStatusCode.InternalServerError,
        ex: ex);
  }

```
