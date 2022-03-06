namespace ToyRobot.Results;

public class RobotPlaceInstructionValidationResult : InputValidationResult
{
    public int X { get; set; }
    public int Y { get; set; }
    public BearingType? BearingType { get; set; }
    
}