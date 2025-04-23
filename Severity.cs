using System.ComponentModel;

namespace ErrOrValue;

public enum Severity
{
  [Description("info")]
  Info,
  [Description("warning")]
  Warning,
  [Description("error")]
  Error
}
