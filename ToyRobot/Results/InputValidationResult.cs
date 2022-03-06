using ToyRobot;
using ToyRobot.Results;

public class InputValidationResult : IInputValidationResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public Command Command { get; set; }
}