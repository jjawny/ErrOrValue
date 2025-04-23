![ErrOrValue](https://raw.githubusercontent.com/jjjjony/ErrOrValue/main/ErrOrValue.png)

# ErrOrValue

A slim object with _[errors as values](https://go.dev/blog/errors-are-values)_ for ASP.NET projects

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

### Why?

TODO:

### Usage

Feel free to copy and paste into your project, add to it, or make it even slimmer!
