![ErrOrValue](https://raw.githubusercontent.com/jjjjony/ErrOrValue/main/ErrOrValue.png)

# ErrOrValue ❌ ✅

The Best _[Errors as Values](https://go.dev/blog/errors-are-values)_ for ASP.NET Projects

## Show me an example?

```csharp
  // Some method
  public ErrOr<bool> Find()
  {
    var response = new ErrOr<bool>();

    try
    {
      // Happy path
      return response.Set(
        message: "Successfully found it",
        severity: Severity.Info,
        code: HttpStatusCode.OK,
        value: true);

      // Issues
      return response.Set(
        message: "Unable to find it",
        severity: Severity.Warning,
        code: HttpStatusCode.NotFound,
        value: false);
    }
    catch (Exception ex)
    {
      // Exceptions
      return response.Set(
        message: "Something went wrong...",
        severity: Severity.Error,
        code: HttpStatusCode.InternalServerError,
        ex: ex);
    }
  }
```

```csharp
  // You can quickly see if the op was successful
  var findResponse = Find();
  Console.Write(findResponse.IsOk)
  
  // ...and safely read the value
  if (findResponse.IsOkWithValue)
    Console.Write(response.Value);
  
  // ...and read any errors
  Console.Write(findResponse.Errors.Count)

  // ...and easily bubble-up
  var response = new ErrOr();
  return response.MergeWith(findResponse);
```

## Why?

Similar packages are usually over-engineered (bloated APIs) when the concept is simple: ***Errors as Values***. This means we need a lightweight object that lets you check if the op was **OK**. OK? access the value safely, not OK? access the error(s). Tailored for ASP.NET Web APIs, these errors come in the form of messages w 'error' severity (can easily become telemetry traces or shown in UI toasts) + easily bubble-up HTTP status codes (vs losing context and just returning a 200 or 500). This package will always be a single file (excluding extension methods) that you can easily copy n modify in your own project (avoid the extra dependency). **Benefits**? perf gains by avoiding throwing exceptions in your code (this keeps exceptions for **unexpected** errors (from the framework or 3rd party libs)), forces devs to consider how to handle each error, explicit code paths for errors, faster debugging (messages act like a mini stack trace), + more!
