using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace ErrOrValue;

public class ErrOr
{
  public HttpStatusCode Code { get; set; } = HttpStatusCode.OK;
  public List<(string Message, Severity Severity)> Messages { get; set; } = [];
  public bool IsOk => (int)Code >= 200 && (int)Code <= 299 && !Messages.Any(m => m.Severity == Severity.Error);

  public IReadOnlyList<string> Errors => Messages
    .Where(m => m.Severity == Severity.Error)
    .Select(m => m.Message)
    .ToList();
}

public class ErrOr<T> : ErrOr
{
  public T? Value { get; set; }

  [MemberNotNullWhen(true, nameof(Value))]
  public bool IsOkWithValue => base.IsOk && Value != null;
}
