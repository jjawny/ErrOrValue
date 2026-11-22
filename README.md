

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

An operation returns either a `Success<T>` or an `Error`. `Success` defaults to HTTP status code 200 and `Error` defaults to HTTP status code 400.

The type of the result determines success, not the message severities/codes.

Below is an example for how to handle the result with more examples in the [sandbox](./ErrOrValue.Sandbox/Program.cs).


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

You can also check for a success/failure using the declaration pattern:


```csharp
if (result is Success<T> success) { }
```

You can chain operations together and propagate the result directly into another result type:

```csharp
static ErrOr<bool> IsAdult(int userId) => Find(userId) switch
{
  Success<string> success => new Success<int>(success.Value.Age > 18),
  Error error => error
};
```

> ℹ️ Because you have control over your business logic, it is recommended for all methods to return an ErrOr. This forces you to handle each error scenario. Google more benefits why errors as values leads to better software.


### ASP.NET responses

Respond from a route/endpoint using:


```csharp
// Controllers
return result.ToApiResponse();

// Minimal API
return result.ToMinimalApiResponse();
```

### HTTP clients

Convert a HTTP response into a result:

```csharp
using var httpResponse = await httpClient.GetAsync(url, cancellationToken);
ErrOr<UserDto> result = await httpResponse.FromHttpResponse<UserDto>("External service", cancellationToken);

// For responses without a JSON value, including HTTP 204:
ErrOr resultWithoutValue = response.FromHttpResponse("External service");
```

### Migrating from v5

.NET 11 (C# 15) has introduced [union types](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/union). Version 6 of ErrOrValue is now built on top of this new feature and has breaking changes.

| v5 | v6 |
| --- | --- |
| .NET 9 | .NET 11 |
| `new ErrOr<T> { Value = value }` | `new Success<T>(value)` |
| `new ErrOr()` | `new Success()` or `new Error(message)` |
| `IsOk` / `IsOkWithValue` | Use native switch statements/expressions or declaration patterns |
| Mutable `Value`, `Code`, `Messages` | Now using [records](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/records); add during creation and use `with` expressions for updates |
| `AddMessage` / `AddMessages` | Now using records |
| `Set(...)` | Return an explicit `success` or `error` |
| `MergeWith(...)` | Pattern match the result and map to a new result type |
| `Errors` | Pattern match the `Error` as an error, then read the messages |
| `errOr.FromHttpResponse<T>(response, service)` | `httpResponse.FromHttpResponse<T>(service)` |
| `ToApiResponse(value)` / `ToMinimalApiResponse(value)` | Put the value in `Success<T>` first |
| `LogToConsole()` | Pattern match the cases and use your choice of logger |


## Test it

Test the [sandbox](./ErrOrValue.Sandbox/Program.cs) using the commands below to find a user given their ID.

```sh
cd ErrOrValue.Sandbox
dotnet run -- 123 # example for success
dotnet run -- 000 # example for error
```
