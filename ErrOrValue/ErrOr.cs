using System.Net;

namespace ErrOrValue;

/// <summary>An operation that either succeeds without a value or fails.</summary>
public union ErrOr(Success, Error);

/// <summary>An operation that either succeeds with a value or fails.</summary>
public union ErrOr<T>(Success<T>, Error);

public sealed record Success(HttpStatusCode Code = HttpStatusCode.OK)
{
  public IReadOnlyList<(string Message, Severity Severity)> Messages { get; init; } = [];
}

public sealed record Success<T>(T Value, HttpStatusCode Code = HttpStatusCode.OK)
{
  public IReadOnlyList<(string Message, Severity Severity)> Messages { get; init; } = [];
}

public sealed record Error(string Message, HttpStatusCode Code = HttpStatusCode.BadRequest)
{
  public IReadOnlyList<(string Message, Severity Severity)> Messages { get; init; } = [];
}
