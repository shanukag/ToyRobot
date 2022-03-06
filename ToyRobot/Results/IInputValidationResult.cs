namespace ToyRobot.Results;

public interface IInputValidationResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public Command Command { get; set; }
}