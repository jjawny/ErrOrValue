![ErrOrValue](https://raw.githubusercontent.com/jjawny/ErrOrValue/main/ErrOrValue.png)

# ErrOrValue ❌✅

The best _[Errors as Values](https://go.dev/blog/errors-are-values)_ for ASP.NET Projects

<details>
<summary>1. Show me an example?</summary>

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

</details>


<details>
<summary>2. Why?</summary>

- Similar packages are over-engineered (heavy-weights with bloated APIs), meanwhile the concept is simple: ***Errors as Values*** (Google it)
- All we need is a lightweight object that lets you check if the operation was successful and either access the value (type)safely or read detailed errors

</details>


<details>
<summary>3. Benefits?</summary>

- Performance gains by avoiding throwing exceptions
- Keeps exceptions for truely unexpected errors that should **not** be in production (from the framework or 3rd party libs)
- Forces devs to explicitly handle each error
- Explicit error handling = new devs grok the code faster
- Messages acting like a mini stack trace = debug problems faster
- Allows you to easily surface context-specific details from deep within the stack trace rather than the usual _"500 Something went wrong..."_

</details>
