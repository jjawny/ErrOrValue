using ErrOrValue;

var errOr = new ErrOr<Dto?>();

// Happy path
var random = new Random();
if (random.Next(2) == 0) // 50% chance
  errOr.Value = new Dto { Name = "Broski" };

errOr.AddMessage("ℹ️", Severity.Info);
errOr.AddMessage("⚠️", Severity.Warning);

Console.WriteLine($"Is OK? {errOr.IsOkWithValue}");
if (errOr.IsOkWithValue)
  Console.WriteLine($"Value? {errOr.Value.Name}");

// Failure path
errOr.AddMessage("❗️", Severity.Error);
Console.WriteLine($"Is still OK? {errOr.IsOk}");

public class Dto
{
  public string Name { get; set; } = null!;
}
