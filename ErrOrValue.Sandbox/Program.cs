using ErrOrValue;

var errOr1 = new ErrOr();
var errOr2 = new ErrOr<string>();
var errOr3 = new ErrOr<DTO?>();

errOr1.AddMessage("ℹ️", Severity.Info);
errOr1.AddMessage("⚠️", Severity.Warning);
errOr1.AddMessage("❗️", Severity.Error);
errOr2.Value = "Broski 1";
errOr3.Value = new DTO { Name = "Broski 2" };

Console.WriteLine(errOr1.IsOk);
Console.WriteLine(errOr1.Errors.Count);
Console.WriteLine(string.Join("\n", errOr1.Errors));


errOr1.LogToConsole();
errOr2.LogToConsole();
errOr3.LogToConsole();

public class DTO
{
  public string Name { get; set; } = null!;
}
