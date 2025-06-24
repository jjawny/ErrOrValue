using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace ErrOrValue;

public class ErrOr
{
  public bool IsOk => !Messages.Any(m => m.Severity == Severity.Error);
  public HttpStatusCode Code { get; set; } = HttpStatusCode.OK;
  public List<(string Message, Severity Severity)> Messages { get; set; } = [];
  public IReadOnlyList<string> Errors => Messages
    .Where(m => m.Severity == Severity.Error)
    .Select(m => m.Message)
    .ToList();
}

public class ErrOr<T> : ErrOr
{
  [MemberNotNullWhen(true, nameof(Value))]
  public bool IsOkWithValue => base.IsOk && Value != null;
  public T? Value { get; set; }
}
