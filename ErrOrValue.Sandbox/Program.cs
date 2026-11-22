using System.Net;
using ErrOrValue;

var userId = 123;

#region Example setup

if (args.Length > 1 || (args.Length == 1 && !int.TryParse(args[0], out userId)))
{
  Console.Error.WriteLine("Usage: dotnet run -- [user-id] (must be an integer)");
  Environment.ExitCode = 1;
  return;
}

#endregion

// EXAMPLES FOR HANDLING THE RESULT
var result = FindUser(userId);

// 1. Use if/else blocks to check the type with 'is'.
if (result is Error error)
{
  Console.WriteLine($"{(int)error.Code}: {error.Message}");
}
else if (result is Success<UserDto> success)
{
  Console.WriteLine($"Found user '{success.Value.Name}'");
}

// 2. Use a switch statement to check the type with a case.
switch (result)
{
  case Success<UserDto> successResult:
    Console.WriteLine($"Found user '{successResult.Value.Name}'");
    break;
  case Error failureResult:
    Console.WriteLine($"{(int)failureResult.Code}: {failureResult.Message}");
    break;
}

// 3. Use a switch expression to quickly assign an outcome based on the result.
var message = result switch
{
  Success<UserDto> successResult => $"Found user '{successResult.Value.Name}'",
  Error failureResult => $"{(int)failureResult.Code}: {failureResult.Message}"
};
Console.WriteLine(message);

#region Example setup

static ErrOr<UserDto> FindUser(int userId) => userId == 123
  ? new Success<UserDto>(new UserDto("Chris P. Bacon"))
  : new Error("User not found", HttpStatusCode.NotFound);

public record UserDto(string Name);

#endregion
