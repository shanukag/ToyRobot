using System.Text;
using System.Text.RegularExpressions;
using ToyRobot.Results;

namespace ToyRobot.Utilities;

public class InputProcessor
{
    private string _input;
    private Dictionary<string, Func<IInputValidationResult>> _dictionary = null;
    private GameStatus GameStatus { get; set; }
    private const string MoveCommand = "MOVE";
    private const string ReportCommand = "REPORT";
    private const string LeftCommand = "LEFT";
    private const string RightCommand = "RIGHT";
    private const string PlaceCommand = "PLACE";
    
    /// <summary>
    /// A utility class to process players' input
    /// </summary>
    /// <param name="input"></param>
    public InputProcessor(GameStatus gameStatus)
    {
        GameStatus = gameStatus;
        _dictionary = new Dictionary<string, Func<IInputValidationResult>>()
        {
            {
                MoveCommand, ProcessMoveInstruction
            },
            {
                PlaceCommand, ProcessPlaceInstruction
            },
            {
                ReportCommand, ProcessReportInstruction
            },
            {
                LeftCommand, ProcessLeftInstruction
            },
            {
                RightCommand, ProcessRightInstruction
            }
        };
    }

    public void SetInput(string input) => _input = input.ToUpper();
    
    public void SetGameStatus(GameStatus gameStatus) => this.GameStatus = gameStatus;

    public IInputValidationResult ValidateInput()
    {
        IInputValidationResult inputValidationResult = new InputValidationResult();
        
        //Identify if the provided input matches the collection of acceptable commands, and execute the defined routine
        foreach (var instruction in _dictionary.Where(instruction => _input.Contains(instruction.Key)))
            inputValidationResult = instruction.Value();

        //If the provided input doesn't match any acceptable command, return an error message with examples for acceptable commands
        if (!inputValidationResult.Success && string.IsNullOrEmpty(inputValidationResult.ErrorMessage))
        {
            StringBuilder builder = new StringBuilder();
            
            builder.Append("Input is in incorrect format. A command can only be in the form of numbers and letters and one of the following :");
            builder.AppendLine();
            builder.Append("To place the robot : Place 3,2,North");
            builder.AppendLine();
            builder.Append("To move the robot : move");
            builder.AppendLine();
            builder.Append("To rotate the robot to the left: left");
            builder.AppendLine();
            builder.Append("To rotate the robot to the right: right");
            builder.AppendLine();
            builder.Append("To get the current location of the robot: report");
            inputValidationResult.ErrorMessage = builder.ToString();
        }
        
        return inputValidationResult;
    }
    
    /// <summary>
    /// Process the input entered to place a robot
    /// Validate to ensure the input is in the correct format Eg : 1,2,North if a robot hasn't been placed yet
    /// If a robot has already been placed, providing of bearing is optional -> Validate bearing only if bearing has been provided
    /// </summary>
    /// <returns></returns>
    public IInputValidationResult ProcessPlaceInstruction()
    {
        Regex rg; 

        RegexOptions regexOptions = RegexOptions.IgnoreCase;
        rg = ShouldValidateBearing() ? new Regex(@"place [0-9]+,[0-9]+,[a-zA-Z]", regexOptions) : new Regex(@"place [0-9]+,[0-9]", regexOptions);
        
        RobotPlaceInstructionValidationResult validationResult = new RobotPlaceInstructionValidationResult();

        if (rg.IsMatch(_input))
        {
            string[] input = _input.Split(" ");
            string [] positionAndBearingArray = input[1].Split(",");

            bool xParseAttempt = Int32.TryParse(positionAndBearingArray[0], out int x);
            bool yParseAttempt = Int32.TryParse(positionAndBearingArray[1], out int y);

            if (!xParseAttempt)
            {
                validationResult.ErrorMessage = "X coordinate is entered in an incorrect format. It can only be a number between 0 - 5";
                return validationResult;
            }
            
            if (!yParseAttempt)
            {
                validationResult.ErrorMessage = "Y coordinate is entered in an incorrect format. It can only be a number between 0 - 5";
                return validationResult;
            }

            if (ShouldValidateBearing(positionAndBearingArray))
            {
                var bearing = ConvertBearing(positionAndBearingArray[2]);
                if (bearing != null)
                {
                    validationResult.Success = true;
                    validationResult.X = x;
                    validationResult.Y = y;
                    validationResult.BearingType = bearing;

                }
                else
                {
                    validationResult.ErrorMessage = "Incorrect direction entered. Direction can only be entered as one of the following : North, East, West or South";
                }    
            }
            else
            {
                validationResult.Success = true;
                validationResult.X = x;
                validationResult.Y = y;
            }
        }
        else
        {
            validationResult.Success = false;
            validationResult.ErrorMessage = "Robot place command is in incorrect format. Please enter command as {X Position},{Y Position},{Direction - North/West/East/South}. Eg : 1,2,North";
        }

        return validationResult;
    }

    //Validate bearing if the game is in initializing stage, or if bearing has been provided
    private bool ShouldValidateBearing(string[] inputArray = null)
    {
        return GameStatus == GameStatus.GameInitialized || inputArray is {Length: > 2};
    }

    /// <summary>
    /// Converts the entered direction to an enum value
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private BearingType? ConvertBearing(string direction)
    {
        BearingType? bearingType = direction switch
        {
            "NORTH" => BearingType.North,
            "EAST" => BearingType.East,
            "SOUTH" => BearingType.South,
            "WEST" => BearingType.West,
            _ => null
        };

        return bearingType;
    }

    /// <summary>
    /// Process the input entered to move a robot
    /// </summary>
    /// <returns></returns>
    public IInputValidationResult ProcessMoveInstruction()
    {
        IInputValidationResult validationResult = new InputValidationResult();

        if (_input.Trim().Equals(MoveCommand))
        {
            validationResult.Command = Command.Move;
            validationResult.Success = true;
        }
        else
        {
            validationResult.ErrorMessage = "Move command is in incorrect format. Please ensure the command is in the format : 'MOVE' ";
        }

        return validationResult;
    }
    
    /// <summary>
    /// Process the input entered to rotate a robot to the left
    /// </summary>
    /// <returns></returns>
    private IInputValidationResult ProcessLeftInstruction()
    {
        RotateCommandValidationResult validationResult = new RotateCommandValidationResult();

        if (_input.Trim().Equals(LeftCommand))
        {
            validationResult.Command = Command.Rotate;
            validationResult.Direction = Direction.Left;
            validationResult.Success = true;
        }
        else
        {
            validationResult.ErrorMessage = "Rotate left command is in incorrect format. Please ensure the command is in the format : 'LEFT' ";
        }

        return validationResult;
    }
    
    /// <summary>
    /// Process the input entered to rotate a robot to the right
    /// </summary>
    /// <returns></returns>
    private IInputValidationResult ProcessRightInstruction()
    {
        RotateCommandValidationResult validationResult = new RotateCommandValidationResult();

        if (_input.Trim().Equals(RightCommand))
        {
            validationResult.Direction = Direction.Right;
            validationResult.Command = Command.Rotate;
            validationResult.Success = true;
        }
        else
        {
            validationResult.ErrorMessage = "Rotate right command is in incorrect format. Please ensure the command is in the format : 'RIGHT'";
        }

        return validationResult;
    }
    
    /// <summary>
    /// Process the input entered to report the current location of a robot
    /// </summary>
    /// <returns></returns>
    private IInputValidationResult ProcessReportInstruction()
    {
        IInputValidationResult validationResult = new InputValidationResult();

        if (_input.Trim().Equals(ReportCommand))
        {
            validationResult.Command = Command.Report;
            validationResult.Success = true;
        }
        else
        {
            validationResult.ErrorMessage = "Report command is in incorrect format. Please ensure the command is in the format : 'REPORT'";
        }

        return validationResult;
    }
}