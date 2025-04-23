using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace ErrOrValue;

public class ErrOr
{
  public HttpStatusCode Code { get; set; } = HttpStatusCode.OK;
  public List<(string Message, Severity Severity)> Messages { get; set; } = [];
  public bool IsOk => (int)Code >= 200 && (int)Code <= 299 && !Messages.Any(m => m.Severity == Severity.Error);
}

public class ErrOr<T> : ErrOr
{
  public T? Value { get; set; }

  [MemberNotNullWhen(true, nameof(Value))]
  public new bool IsOk => base.IsOk && Value != null;
}
