using System.Net;
using Xunit;

namespace ErrOrValue.Tests;

public class UnionTests
{
  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void MatchesBothGenericCases(bool succeeds)
  {
    ErrOr<int> result = succeeds ? new Success<int>(42) : new Error("Failed");

    var message = result switch
    {
      Success<int> success => success.Value.ToString(),
      Error error => error.Message,
      null => "Uninitialized",
    };

    Assert.Equal(succeeds ? "42" : "Failed", message);
  }

  [Fact]
  public void ErrorCanPropagateBetweenDifferentValueTypes()
  {
    ErrOr<string> source = new Error("Missing", HttpStatusCode.NotFound);
    ErrOr<int> target = source switch
    {
      Success<string> success => new Success<int>(success.Value.Length),
      Error error => error,
      null => throw new InvalidOperationException(),
    };

    Assert.True(target is Error { Message: "Missing", Code: HttpStatusCode.NotFound });
  }

  [Fact]
  public void NullableSuccessRemainsDistinctFromUninitializedUnion()
  {
    ErrOr<string?> result = new Success<string?>(null);
    Assert.True(result is Success<string?> { Value: null });
    Assert.False(result is null);
    Assert.True(default(ErrOr<string?>) is null);
  }

  [Fact]
  public void SuccessWithoutValueIsExplicit()
  {
    ErrOr result = new Success(HttpStatusCode.Accepted);
    Assert.True(result is Success { Code: HttpStatusCode.Accepted });

    result = new Error("Failed");
    Assert.True(result is Error);
  }
}
