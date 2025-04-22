# ❌ ErrOrResult

A slim object for _[errors as values](https://go.dev/blog/errors-are-values)_ in ASP.NET

### Example

```csharp

  var res = new ErrOr<bool>();

  try
  {
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

  }
  catch (Exception ex)
  {
    // Exceptions
    return res.QuickReturn(
      message: "Something went wrong...",
      severity: Severity.Error,
      code: HttpStatusCode.InternalServerError,
      ex: ex);
  }

```

### Usage

Feel free to copy and paste into your project, add to it, or make it even slimmer!
