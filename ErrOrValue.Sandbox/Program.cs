using ErrOrValue;

var errOr1 = new ErrOr();
var errOr2 = new ErrOr<string>();
var errOr3 = new ErrOr<ExampleDto>();

errOr1.AddMessage("test message", Severity.Info);
errOr2.Value = "Broski 1";
errOr3.Value = new ExampleDto { Name = "Broski 2" };

errOr1.LogToConsole();
errOr2.LogToConsole();
errOr3.LogToConsole();

public class ExampleDto
{
  public string Name { get; set; } = null!;
}
