![ErrOrValue](https://raw.githubusercontent.com/jjjjony/ErrOrValue/main/ErrOrValue.png)

# ErrOrValue ❌✅

A slim object with _[errors as values](https://go.dev/blog/errors-are-values)_ for ASP.NET projects

### Example

```csharp
  // Some method
  public ErrOr<bool> Find()
  {
    var res = new ErrOr<bool>();

    try
    {
      // Happy path
      return res.Set(
        value: true,
        message: "Successfully found it",
        severity: Severity.Info,
        code: HttpStatusCode.OK);

      // Issues
      return res.Set(
        value: false,
        message: "Unable to find it",
        severity: Severity.Warning,
        code: HttpStatusCode.NotFound);
    }
    catch (Exception ex)
    {
      // Exceptions
      return res.Set(
        message: "Something went wrong...",
        severity: Severity.Error,
        code: HttpStatusCode.InternalServerError,
        ex: ex);
    }
  }
```

```csharp
  // You can safely access the value if everything is okay
  var res = Find();

  if (res.IsOk)
  {
    Console.WriteLine(res.Value);
  }
```

### Why?

TLDR Most similar packages have overly verbose APIs, less is more, especially when the idea is to use this type everywhere. I even recommend copying this into your project locally rather than using it as a NuGet dependency so you can have full control over it!
