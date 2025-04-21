using System.ComponentModel;

namespace ErrOrResult;

public enum Severity
{
  [Description("info")]
  Info,
  [Description("warning")]
  Warning,
  [Description("error")]
  Error
}
