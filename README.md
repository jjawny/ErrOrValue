

<div align="center">

# ErrOrValue

![ErrOrValue](https://raw.githubusercontent.com/jjawny/ErrOrValue/main/ErrOrValue.png)

**The best [errors as values](https://go.dev/blog/errors-are-values) for ASP.NET projects**

[Set it up](#set-it-up) · [Use it](#use-it) · [Test it](#test-it)

</div>

## Set it up

```sh
dotnet add package ErrOrValue
```

> ℹ️ The latest version of ErrOrValue requires [.NET 11](https://dotnet.microsoft.com/en-us/download/dotnet/11.0).

## Use it

An operation returns either a `Success<T>` or an `Error`. Operations without a return value use `Success`. Success cases default to HTTP status code 200, and errors default to HTTP status code 400.

The type of the result determines whether the operation was successful. Message severities and HTTP status codes do not change the case.

The following example shows how to handle a result. Find more examples in the [sandbox](https://github.com/jjawny/ErrOrValue/blob/main/ErrOrValue.Sandbox/Program.cs).

```csharp
using ErrOrValue;

// This method returns an error or UserDto
static ErrOr<UserDto> FindUser(int userId) => userId == 123
  ? new Success<UserDto>(new UserDto("Chris P. Bacon"))
  : new Error("User not found", HttpStatusCode.NotFound);

// Handle the error or UserDto
var result = FindUser(123);

Console.WriteLine(result switch
{
  Success<UserDto> success => $"Found user '{success.Value.Name}'",
  Error error => $"{(int)error.Code}: {error.Message}"
});

```

You can also check for success or failure using the declaration pattern:

```csharp
if (result is Success<T> success) { }
```

You can chain operations together. Transform a success value to another type or propagate errors directly:

```csharp
static ErrOr<bool> IsAdult(int userId) => FindUser(userId) switch
{
  Success<UserDto> success => new Success<bool>(success.Value.Age >= 18),
  Error error => error
};
```

### ASP.NET responses

Return a response from a controller or minimal API endpoint:

```csharp
// Controllers
return result.ToApiResponse();

// Minimal API
return result.ToMinimalApiResponse();
```

### HTTP clients

Convert an HTTP response into a result:

```csharp
using var httpResponse = await httpClient.GetAsync(url, cancellationToken);
ErrOr<UserDto> result = await httpResponse.FromHttpResponse<UserDto>("External service", cancellationToken);

// For responses without a JSON value, including HTTP 204:
ErrOr resultWithoutValue = httpResponse.FromHttpResponse("External service");
```

### Migrating from v5

.NET 11 (C# 15) has introduced [union types](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/union). Version 6 of ErrOrValue is now built on top of this new feature and has breaking changes.

| v5 | v6 |
| --- | --- |
| .NET 9 | .NET 11 |
| `new ErrOr<T> { Value = value }` | `new Success<T>(value)` |
| `new ErrOr()` | `new Success()` or `new Error(message)` |
| `IsOk` / `IsOkWithValue` | Use native switch statements/expressions or declaration patterns |
| Mutable `Value`, `Code`, `Messages` | Now using [records](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/records); initialize properties during creation and use `with` expressions for updates |
| `AddMessage` / `AddMessages` | Initialize the `Messages` during creation |
| `Set(...)` | Return an explicit `Success`, `Success<T>`, or `Error` case |
| `MergeWith(...)` | Pattern match the result and map to a new result type |
| `Errors` | Pattern match the `Error` case, then read the messages |
| `errOr.FromHttpResponse<T>(response, service)` | `httpResponse.FromHttpResponse<T>(service)` |
| `ToApiResponse(value)` / `ToMinimalApiResponse(value)` | Put the value in `Success<T>` first |
| `LogToConsole()` | Pattern match the cases and use your choice of logger |

## Test it

Test the [sandbox](https://github.com/jjawny/ErrOrValue/blob/main/ErrOrValue.Sandbox/Program.cs) using the commands below to find a user given their ID.

```sh
cd ErrOrValue.Sandbox
dotnet run -- 123 # Success example
dotnet run -- 000 # Error example
```
